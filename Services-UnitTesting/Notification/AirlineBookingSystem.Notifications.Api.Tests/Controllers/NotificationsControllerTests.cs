using AirlineBookingSystem.Notifications.Api.Controllers;
using AirlineBookingSystem.Notifications.Application.Commands;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace AirlineBookingSystem.Notifications.Api.Tests.Controllers;

public class NotificationsControllerTests
{
    private readonly Mock<IMediator> _mediatorMock = new();
    private readonly NotifcationsController _controller;

    public NotificationsControllerTests()
    {
        _controller = new NotifcationsController(_mediatorMock.Object);
    }

    [Fact]
    public async Task SendNotification_ShouldSendCommandAndReturnOk()
    {
        // Arrange
        var command = new SendNotificationCommand("user@example.com", "Test message", "Email");

        _mediatorMock
            .Setup(m => m.Send(It.Is<SendNotificationCommand>(c => c == command), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask)
            .Verifiable();

        // Act
        var result = await _controller.SendNotification(command);

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.Value.Should().Be("Notification sent successfully");

        _mediatorMock.Verify(m => m.Send(It.Is<SendNotificationCommand>(c => c == command), It.IsAny<CancellationToken>()), Times.Once);
    }
}
