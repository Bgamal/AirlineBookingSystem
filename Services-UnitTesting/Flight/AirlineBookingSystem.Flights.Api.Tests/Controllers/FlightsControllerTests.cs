using AirlineBookingSystem.Fights.Api.Controllers;
using AirlineBookingSystem.Fights.Application.Commands;
using AirlineBookingSystem.Fights.Application.Queries;
using AirlineBookingSystem.Fights.Core.Entities;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace AirlineBookingSystem.Flights.Api.Tests.Controllers;

public class FlightsControllerTests
{
    private readonly Mock<IMediator> _mediatorMock = new();
    private readonly FlightsController _controller;

    public FlightsControllerTests()
    {
        _controller = new FlightsController(_mediatorMock.Object);
    }

    [Fact]
    public async Task GetFlights_WhenMediatorReturnsFlights_ShouldReturnOk()
    {
        // Arrange
        var flights = new List<Flight>
        {
            new()
            {
                Id = Guid.NewGuid(),
                FlightNumber = "AB123",
                Origin = "Cairo",
                Destination = "Riyadh",
                DepartureTime = DateTime.UtcNow,
                ArrivalTime = DateTime.UtcNow.AddHours(2)
            }
        };

        _mediatorMock
            .Setup(m => m.Send(It.IsAny<GatAllFlightsQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(flights);

        // Act
        var result = await _controller.GetFlights();

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.Value.Should().BeEquivalentTo(flights);

        _mediatorMock.Verify(m => m.Send(It.IsAny<GatAllFlightsQuery>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task AddFlight_WhenMediatorReturnsId_ShouldReturnCreatedAtAction()
    {
        // Arrange
        var command = new CreateFlightCommand("AB456", "Riyadh", "Dubai", DateTime.UtcNow, DateTime.UtcNow.AddHours(3));
        var flightId = Guid.NewGuid();

        _mediatorMock
            .Setup(m => m.Send(It.Is<CreateFlightCommand>(c => c == command), It.IsAny<CancellationToken>()))
            .ReturnsAsync(flightId);

        // Act
        var result = await _controller.AddFlight(command);

        // Assert
        var createdResult = result.Should().BeOfType<CreatedAtActionResult>().Subject;
        createdResult.ActionName.Should().Be(nameof(FlightsController.GetFlights));
        createdResult.RouteValues.Should().ContainKey("id").WhoseValue.Should().Be(flightId);
        createdResult.Value.Should().Be(command);

        _mediatorMock.Verify(m => m.Send(It.Is<CreateFlightCommand>(c => c == command), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteFlight_ShouldSendCommandAndReturnNoContent()
    {
        // Arrange
        var flightId = Guid.NewGuid();

        // Act
        var result = await _controller.DeleteFlight(flightId);

        // Assert
        result.Should().BeOfType<NoContentResult>();
        _mediatorMock.Verify(m => m.Send(It.Is<DeleteFlightCommand>(c => c.Id == flightId), It.IsAny<CancellationToken>()), Times.Once);
    }
}
