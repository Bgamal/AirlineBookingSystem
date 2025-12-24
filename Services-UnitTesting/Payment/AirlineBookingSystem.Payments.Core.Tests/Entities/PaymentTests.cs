using AirlineBookingSystem.Payments.Core.Entities;
using FluentAssertions;
using Xunit;

namespace AirlineBookingSystem.Payments.Core.Tests.Entities;

public class PaymentTests
{
    [Fact]
    public void Constructor_ShouldAllowPropertyInitialization()
    {
        // Arrange
        var id = Guid.NewGuid();
        var bookingId = Guid.NewGuid();
        var paymentDate = new DateTime(2025, 1, 1, 10, 0, 0, DateTimeKind.Utc);

        // Act
        var payment = new Payment
        {
            Id = id,
            BookingId = bookingId,
            Amount = 250m,
            Balance = 0m,
            PaymentDate = paymentDate
        };

        // Assert
        payment.Id.Should().Be(id);
        payment.BookingId.Should().Be(bookingId);
        payment.Amount.Should().Be(250m);
        payment.Balance.Should().Be(0m);
        payment.PaymentDate.Should().Be(paymentDate);
    }
}
