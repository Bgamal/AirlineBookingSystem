using AirlineBookingSystem.Notifications.Application.Commands;
using AirlineBookingSystem.Notifications.Application.Handlers;
using AirlineBookingSystem.Notifications.Application.Interfaces;
using AirlineBookingSystem.Notifications.Core.Entities;
using FluentAssertions;
using Moq;
using Xunit;

namespace AirlineBookingSystem.Notifications.Application.Tests.Handlers;

public class SendNotificationHandlerTests
{
    private readonly Mock<INotificationService> _serviceMock = new();
    private readonly SendNotificationHandler _handler;

    public SendNotificationHandlerTests()
    {
        _handler = new SendNotificationHandler(_serviceMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldCreateNotificationAndSendIt()
    {
        // Arrange
        Notification? capturedNotification = null;
        _serviceMock
            .Setup(s => s.SendNotificationAsync(It.IsAny<Notification>()))
            .Callback<Notification>(n => capturedNotification = n)
            .Returns(Task.CompletedTask);

        var command = new SendNotificationCommand("user@example.com", "Hello there", "Email");

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        capturedNotification.Should().NotBeNull();
        capturedNotification!.Id.Should().NotBeEmpty();
        capturedNotification.Recipient.Should().Be(command.Recipient);
        capturedNotification.Message.Should().Be(command.Message);
        capturedNotification.Type.Should().Be(command.Type);

        _serviceMock.Verify(s => s.SendNotificationAsync(It.IsAny<Notification>()), Times.Once);
    }
}
