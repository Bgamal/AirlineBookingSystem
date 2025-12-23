using AirlineBookingSystem.Bookings.Api.Controllers;
using AirlineBookingSystem.Bookings.Application.Commands;
using AirlineBookingSystem.Bookings.Application.Queries;
using AirlineBookingSystem.Bookings.Core.Entities;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace AirlineBookingSystem.Bookings.Api.Tests.Controllers;

public class BookingControllerTests
{
    private readonly Mock<IMediator> _mediatorMock = new();
    private readonly BookingController _controller;

    public BookingControllerTests()
    {
        _controller = new BookingController(_mediatorMock.Object);
    }

    [Fact]
    public async Task AddBooking_WhenMediatorReturnsId_ShouldReturnCreatedAtAction()
    {
        // Arrange
        var bookingId = Guid.NewGuid();
       
        var command = new CreateBookingCommand(Guid.NewGuid(), "Jane Doe", "12A");

        _mediatorMock
            .Setup(m => m.Send(It.Is<CreateBookingCommand>(c => c == command), It.IsAny<CancellationToken>()))
            .ReturnsAsync(bookingId);

        // Act
        var result = await _controller.AddBooking(command);

        // Assert
        var createdResult = result.Should().BeOfType<CreatedAtActionResult>().Subject;
        createdResult.ActionName.Should().Be(nameof(BookingController.GetBookingById));
        createdResult.RouteValues.Should().ContainKey("id").WhoseValue.Should().Be(bookingId);
        createdResult.Value.Should().Be(command);

        _mediatorMock.Verify(m => m.Send(It.Is<CreateBookingCommand>(c => c == command), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetBookingById_WhenBookingExists_ShouldReturnOkWithBooking()
    {
        // Arrange
        var bookingId = Guid.NewGuid();
        var booking = new Booking
        {
            Id = bookingId,
            FlightId = Guid.NewGuid(),
            PassengerName = "John Smith",
            SeatNumber = "8C",
            BookingDate = DateTime.UtcNow
        };

        _mediatorMock
            .Setup(m => m.Send(It.Is<GetBookingQuery>(q => q.Id == bookingId), It.IsAny<CancellationToken>()))
            .ReturnsAsync(booking);

        // Act
        var result = await _controller.GetBookingById(bookingId);

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.Value.Should().BeEquivalentTo(booking);

        _mediatorMock.Verify(m => m.Send(It.Is<GetBookingQuery>(q => q.Id == bookingId), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetBookingById_WhenBookingMissing_ShouldReturnNotFound()
    {
        // Arrange
        var bookingId = Guid.NewGuid();

        _mediatorMock
            .Setup(m => m.Send(It.Is<GetBookingQuery>(q => q.Id == bookingId), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Booking?)null);

        // Act
        var result = await _controller.GetBookingById(bookingId);

        // Assert
        result.Should().BeOfType<NotFoundResult>();

        _mediatorMock.Verify(m => m.Send(It.Is<GetBookingQuery>(q => q.Id == bookingId), It.IsAny<CancellationToken>()), Times.Once);
    }
}
