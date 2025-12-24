using AirlineBookingSystem.Fights.Application.Commands;
using AirlineBookingSystem.Fights.Application.Handlers;
using AirlineBookingSystem.Fights.Core.Entities;
using AirlineBookingSystem.Fights.Core.Repositories;
using FluentAssertions;
using Moq;
using Xunit;

namespace AirlineBookingSystem.Flights.Application.Tests.Handlers;

public class CreateFlightHandlerTests
{
    private readonly Mock<IFlightRepository> _repositoryMock = new();
    private readonly CreateFlightHandler _handler;

    public CreateFlightHandlerTests()
    {
        _handler = new CreateFlightHandler(_repositoryMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldPersistFlightAndReturnGeneratedId()
    {
        // Arrange
        Flight? capturedFlight = null;
        _repositoryMock
            .Setup(r => r.AddFlightAsync(It.IsAny<Flight>()))
            .Callback<Flight>(f => capturedFlight = f)
            .Returns(Task.CompletedTask);

        var command = new CreateFlightCommand(
            "AB123",
            "Cairo",
            "Riyadh",
            DateTime.UtcNow,
            DateTime.UtcNow.AddHours(2));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeEmpty();
        capturedFlight.Should().NotBeNull();
        capturedFlight!.Id.Should().Be(result);
        capturedFlight.FlightNumber.Should().Be(command.FlightNumber);
        capturedFlight.Origin.Should().Be(command.Origin);
        capturedFlight.Destination.Should().Be(command.Destination);
        capturedFlight.DepartureTime.Should().Be(command.DepartureTime);
        capturedFlight.ArrivalTime.Should().Be(command.ArrivalTime);

        _repositoryMock.Verify(r => r.AddFlightAsync(It.IsAny<Flight>()), Times.Once);
    }
}
