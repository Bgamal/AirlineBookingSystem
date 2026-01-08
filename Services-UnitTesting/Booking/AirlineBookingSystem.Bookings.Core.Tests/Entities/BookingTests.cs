using AirlineBookingSystem.Bookings.Core.Entities;
using FluentAssertions;
using Xunit;

namespace AirlineBookingSystem.Bookings.Core.Tests.Entities;

public class BookingTests
{
    [Fact]
    public void Constructor_ShouldAllowPropertyInitialization()
    {
        // Arrange
        var id = Guid.NewGuid();
        var flightId = Guid.NewGuid();
        var passengerName = "John Doe";
        var seatNumber = "15B";
        var bookingDate = new DateTime(2025, 12, 23, 12, 0, 0, DateTimeKind.Utc);

        // Act
        var booking = new Booking
        {
            Id = id,
            FlightId = flightId,
            PassengerName = passengerName,
            SeatNumber = seatNumber,
            BookingDate = bookingDate
        };

        // Assert
        booking.Id.Should().Be(id);
        booking.FlightId.Should().Be(flightId);
        booking.PassengerName.Should().Be(passengerName);
        booking.SeatNumber.Should().Be(seatNumber);
        booking.BookingDate.Should().Be(bookingDate);
    }

    [Fact]
    public void BookingDate_ShouldSupportUtcKind()
    {
        // Arrange
        var booking = new Booking();
        var utcDate = DateTime.UtcNow;

        // Act
        booking.BookingDate = utcDate;

        // Assert
        booking.BookingDate.Kind.Should().Be(DateTimeKind.Utc);
    }
}
