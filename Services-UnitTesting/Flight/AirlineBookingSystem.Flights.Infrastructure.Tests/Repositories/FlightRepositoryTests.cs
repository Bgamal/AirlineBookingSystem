using AirlineBookingSystem.Fights.Core.Entities;
using AirlineBookingSystem.Fights.Infrastructure.Repositories;
using Dapper;
using FluentAssertions;
using Microsoft.Data.Sqlite;
using Xunit;

namespace AirlineBookingSystem.Flights.Infrastructure.Tests.Repositories;

public class FlightRepositoryTests
{
    static FlightRepositoryTests()
    {
        SqlMapper.AddTypeHandler(new SqliteGuidTypeHandler());
    }

    [Fact]
    public async Task AddFlightAsync_ShouldPersistFlight()
    {
        // Arrange
        await using var connection = CreateOpenConnection();
        var repository = new FlightRepository(connection);
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
        var stored = await connection.QuerySingleOrDefaultAsync<Flight>(
            "SELECT * FROM Flights WHERE Id = @Id",
            new { flight.Id });

        stored.Should().NotBeNull();
        stored!.Should().BeEquivalentTo(flight, opts => opts
            .Using<DateTime>(ctx => ctx.Subject.Should().BeCloseTo(ctx.Expectation, TimeSpan.FromSeconds(1)))
            .WhenTypeIs<DateTime>());
    }

    [Fact]
    public async Task GetFlightByIdAsync_WhenExists_ShouldReturnFlight()
    {
        // Arrange
        await using var connection = CreateOpenConnection();
        var repository = new FlightRepository(connection);

        var flight = new Flight
        {
            Id = Guid.NewGuid(),
            FlightNumber = "AB456",
            Origin = "Riyadh",
            Destination = "Dubai",
            DepartureTime = DateTime.UtcNow,
            ArrivalTime = DateTime.UtcNow.AddHours(3)
        };

        await connection.ExecuteAsync(
            "INSERT INTO Flights (Id, FlightNumber, Origin, Destination, DepartureTime, ArrivalTime) VALUES (@Id, @FlightNumber, @Origin, @Destination, @DepartureTime, @ArrivalTime)",
            flight);

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
        await using var connection = CreateOpenConnection();
        var repository = new FlightRepository(connection);

        // Act
        var result = await repository.GetFlightByIdAsync(Guid.NewGuid());

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task DeleteFlightAsync_ShouldRemoveFlight()
    {
        // Arrange
        await using var connection = CreateOpenConnection();
        var repository = new FlightRepository(connection);

        var flight = new Flight
        {
            Id = Guid.NewGuid(),
            FlightNumber = "AB789",
            Origin = "Dubai",
            Destination = "Cairo",
            DepartureTime = DateTime.UtcNow,
            ArrivalTime = DateTime.UtcNow.AddHours(4)
        };

        await connection.ExecuteAsync(
            "INSERT INTO Flights (Id, FlightNumber, Origin, Destination, DepartureTime, ArrivalTime) VALUES (@Id, @FlightNumber, @Origin, @Destination, @DepartureTime, @ArrivalTime)",
            flight);

        // Act
        await repository.DeleteFlightAsync(flight.Id);

        // Assert
        var result = await connection.QuerySingleOrDefaultAsync<Flight>(
            "SELECT * FROM Flights WHERE Id = @Id",
            new { flight.Id });

        result.Should().BeNull();
    }

    private static SqliteConnection CreateOpenConnection()
    {
        var connection = new SqliteConnection("Data Source=:memory:;Mode=Memory;Cache=Shared");
        connection.Open();

        connection.Execute(@"CREATE TABLE IF NOT EXISTS Flights (
            Id TEXT PRIMARY KEY,
            FlightNumber TEXT NOT NULL,
            Origin TEXT NOT NULL,
            Destination TEXT NOT NULL,
            DepartureTime TEXT NOT NULL,
            ArrivalTime TEXT NOT NULL
        )");

        return connection;
    }
}
