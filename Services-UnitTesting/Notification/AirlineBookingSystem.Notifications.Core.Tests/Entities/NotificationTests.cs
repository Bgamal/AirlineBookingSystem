using AirlineBookingSystem.Notifications.Core.Entities;
using FluentAssertions;
using Xunit;

namespace AirlineBookingSystem.Notifications.Core.Tests.Entities;

public class NotificationTests
{
    [Fact]
    public void Constructor_ShouldAllowPropertyInitialization()
    {
        // Arrange
        var id = Guid.NewGuid();

        // Act
        var notification = new Notification
        {
            Id = id,
            Recipient = "user@example.com",
            Message = "Hello",
            Type = "Email"
        };

        // Assert
        notification.Id.Should().Be(id);
        notification.Recipient.Should().Be("user@example.com");
        notification.Message.Should().Be("Hello");
        notification.Type.Should().Be("Email");
    }
}
