using AirlineBookingSystem.Bookings.Application.Handlers;
using AirlineBookingSystem.Bookings.Application.Queries;
using AirlineBookingSystem.Bookings.Core.Entities;
using AirlineBookingSystem.Bookings.Core.Repositories;
using FluentAssertions;
using Moq;
using Xunit;

namespace AirlineBookingSystem.Bookings.Application.Tests.Handlers;

public class GetBookingHandlerTests
{
    private readonly Mock<IBookingRepository> _repositoryMock = new();
    private readonly GetBookingHandler _handler;

    public GetBookingHandlerTests()
    {
        _handler = new GetBookingHandler(_repositoryMock.Object);
    }

    [Fact]
    public async Task Handle_WhenBookingExists_ShouldReturnBooking()
    {
        // Arrange
        var bookingId = Guid.NewGuid();
        var booking = new Booking
        {
            Id = bookingId,
            FlightId = Guid.NewGuid(),
            PassengerName = "Clark Kent",
            SeatNumber = "3A",
            BookingDate = DateTime.UtcNow
        };

        _repositoryMock
            .Setup(r => r.GetBookingByIdAsync(bookingId))
            .ReturnsAsync(booking);

        var query = new GetBookingQuery(bookingId);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().BeEquivalentTo(booking);
        _repositoryMock.Verify(r => r.GetBookingByIdAsync(bookingId), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenBookingMissing_ShouldReturnNull()
    {
        // Arrange
        var bookingId = Guid.NewGuid();
        _repositoryMock
            .Setup(r => r.GetBookingByIdAsync(bookingId))
            .ReturnsAsync((Booking?)null);

        var query = new GetBookingQuery(bookingId);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().BeNull();
        _repositoryMock.Verify(r => r.GetBookingByIdAsync(bookingId), Times.Once);
    }
}
