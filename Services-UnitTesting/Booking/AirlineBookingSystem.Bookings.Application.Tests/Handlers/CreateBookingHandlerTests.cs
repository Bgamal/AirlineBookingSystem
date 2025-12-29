using AirlineBookingSystem.Bookings.Application.Commands;
using AirlineBookingSystem.Bookings.Application.Handlers;
using AirlineBookingSystem.Bookings.Core.Entities;
using AirlineBookingSystem.Bookings.Core.Repositories;
using FluentAssertions;
using MassTransit;
using Moq;
using Xunit;

namespace AirlineBookingSystem.Bookings.Application.Tests.Handlers;

public class CreateBookingHandlerTests
{
    private readonly Mock<IBookingRepository> _repositoryMock = new();
    private readonly Mock<IPublishEndpoint> _publishEndpointMock = new();
    private readonly CreateBookingHandler _handler;

    public CreateBookingHandlerTests()
    {
        _publishEndpointMock
            .Setup(p => p.Publish(It.IsAny<object>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _handler = new CreateBookingHandler(_repositoryMock.Object, _publishEndpointMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldPersistBookingAndReturnGeneratedId()
    {
        // Arrange
        Booking? capturedBooking = null;
        _repositoryMock
            .Setup(r => r.AddBookingAsync(It.IsAny<Booking>()))
            .Callback<Booking>(b => capturedBooking = b)
            .Returns(Task.CompletedTask);

        var command = new CreateBookingCommand(Guid.NewGuid(), "Alice Smith", "21C");

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeEmpty();
        capturedBooking.Should().NotBeNull();
        capturedBooking!.Id.Should().Be(result);
        capturedBooking.FlightId.Should().Be(command.FlightId);
        capturedBooking.PassengerName.Should().Be(command.PassengerName);
        capturedBooking.SeatNumber.Should().Be(command.SeatNumber);
        capturedBooking.BookingDate.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(2));

        _repositoryMock.Verify(r => r.AddBookingAsync(It.IsAny<Booking>()), Times.Once);
    }
}
