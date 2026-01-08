using AirlineBookingSystem.Fights.Core.Entities;
using AirlineBookingSystem.Fights.Infrastructure.Data;
using AirlineBookingSystem.Fights.Infrastructure.Repositories;
using FluentAssertions;
using MongoDB.Driver;
using Moq;
using Xunit;

namespace AirlineBookingSystem.Flights.Infrastructure.Tests.Repositories;

public class FlightRepositoryTests
{
    private Mock<IFlightContext> CreateMockFlightContext()
    {
        return new Mock<IFlightContext>();
    }

    private IMongoCollection<Flight> CreateMongoCollection()
    {
        var client = new MongoClient("mongodb://localhost:27017");
        var database = client.GetDatabase("TestFlightDb");
        database.DropCollection("Flights");
        return database.GetCollection<Flight>("Flights");
    }

    [Fact]
    public async Task AddFlightAsync_ShouldPersistFlight()
    {
        // Arrange
        var collection = CreateMongoCollection();
        var mockContext = new Mock<IFlightContext>();
        mockContext.Setup(x => x.Flights).Returns(collection);

        var repository = new FlightRepository(mockContext.Object);
        var flight = new Flight
        {
            Id = Guid.NewGuid(),
            FlightNumber = "AB123",
            Origin = "Cairo",
            Destination = "Riyadh",
            DepartureTime = DateTime.UtcNow,
            ArrivalTime = DateTime.UtcNow.AddHours(2)
        };

        // Act
        await repository.AddFlightAsync(flight);

        // Assert
        var stored = await collection.Find(f => f.Id == flight.Id).FirstOrDefaultAsync();
        stored.Should().NotBeNull();
        stored!.Should().BeEquivalentTo(flight, opts => opts
            .Using<DateTime>(ctx => ctx.Subject.Should().BeCloseTo(ctx.Expectation, TimeSpan.FromSeconds(1)))
            .WhenTypeIs<DateTime>());
    }

    [Fact]
    public async Task GetFlightByIdAsync_WhenExists_ShouldReturnFlight()
    {
        // Arrange
        var collection = CreateMongoCollection();
        var mockContext = new Mock<IFlightContext>();
        mockContext.Setup(x => x.Flights).Returns(collection);

        var flight = new Flight
        {
            Id = Guid.NewGuid(),
            FlightNumber = "AB456",
            Origin = "Riyadh",
            Destination = "Dubai",
            DepartureTime = DateTime.UtcNow,
            ArrivalTime = DateTime.UtcNow.AddHours(3)
        };

        await collection.InsertOneAsync(flight);

        var repository = new FlightRepository(mockContext.Object);

        // Act
        var result = await repository.GetFlightByIdAsync(flight.Id);

        // Assert
        result.Should().NotBeNull();
        result!.Should().BeEquivalentTo(flight, opts => opts
            .Using<DateTime>(ctx => ctx.Subject.Should().BeCloseTo(ctx.Expectation, TimeSpan.FromSeconds(1)))
            .WhenTypeIs<DateTime>());
    }

    [Fact]
    public async Task GetFlightByIdAsync_WhenMissing_ShouldReturnNull()
    {
        // Arrange
        var collection = CreateMongoCollection();
        var mockContext = new Mock<IFlightContext>();
        mockContext.Setup(x => x.Flights).Returns(collection);

        var repository = new FlightRepository(mockContext.Object);

        // Act
        var result = await repository.GetFlightByIdAsync(Guid.NewGuid());

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task DeleteFlightAsync_ShouldRemoveFlight()
    {
        // Arrange
        var collection = CreateMongoCollection();
        var mockContext = new Mock<IFlightContext>();
        mockContext.Setup(x => x.Flights).Returns(collection);

        var flight = new Flight
        {
            Id = Guid.NewGuid(),
            FlightNumber = "AB789",
            Origin = "Dubai",
            Destination = "Cairo",
            DepartureTime = DateTime.UtcNow,
            ArrivalTime = DateTime.UtcNow.AddHours(4)
        };

        await collection.InsertOneAsync(flight);

        var repository = new FlightRepository(mockContext.Object);

        // Act
        await repository.DeleteFlightAsync(flight.Id);

        // Assert
        var result = await collection.Find(f => f.Id == flight.Id).FirstOrDefaultAsync();
        result.Should().BeNull();
    }
}
