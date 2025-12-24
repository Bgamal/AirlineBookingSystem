using AirlineBookingSystem.Fights.Application.Handlers;
using AirlineBookingSystem.Fights.Application.Queries;
using AirlineBookingSystem.Fights.Core.Entities;
using AirlineBookingSystem.Fights.Core.Repositories;
using FluentAssertions;
using Moq;
using Xunit;

namespace AirlineBookingSystem.Flights.Application.Tests.Handlers;

public class GetAllFlightsHandlerTests
{
    private readonly Mock<IFlightRepository> _repositoryMock = new();
    private readonly GetAllFlightsHandler _handler;

    public GetAllFlightsHandlerTests()
    {
        _handler = new GetAllFlightsHandler(_repositoryMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnFlightsFromRepository()
    {
        // Arrange
        var flights = new List<Flight>
        {
            new()
            {
                Id = Guid.NewGuid(),
                FlightNumber = "AB321",
                Origin = "Cairo",
                Destination = "Jeddah",
                DepartureTime = DateTime.UtcNow,
                ArrivalTime = DateTime.UtcNow.AddHours(2)
            }
        };

        _repositoryMock
            .Setup(r => r.GetAllFlightsAsync())
            .ReturnsAsync(flights);

        var query = new GatAllFlightsQuery();

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().BeEquivalentTo(flights);
        _repositoryMock.Verify(r => r.GetAllFlightsAsync(), Times.Once);
    }
}
