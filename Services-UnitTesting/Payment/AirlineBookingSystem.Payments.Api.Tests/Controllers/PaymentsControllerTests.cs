using AirlineBookingSystem.Payments.Api.Controllers;
using AirlineBookingSystem.Payments.Application.Commands;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace AirlineBookingSystem.Payments.Api.Tests.Controllers;

public class PaymentsControllerTests
{
    private readonly Mock<IMediator> _mediatorMock = new();
    private readonly PaymentsController _controller;

    public PaymentsControllerTests()
    {
        _controller = new PaymentsController(_mediatorMock.Object);
    }

    [Fact]
    public async Task ProcessPayment_WhenMediatorReturnsId_ShouldReturnCreatedAtAction()
    {
        // Arrange
        var command = new ProcessPaymentCommand(Guid.NewGuid(), 250m);
        var paymentId = Guid.NewGuid();

        _mediatorMock
            .Setup(m => m.Send(It.Is<ProcessPaymentCommand>(c => c == command), It.IsAny<CancellationToken>()))
            .ReturnsAsync(paymentId);

        // Act
        var result = await _controller.ProcessPayment(command);

        // Assert
        var createdResult = result.Should().BeOfType<CreatedAtActionResult>().Subject;
        createdResult.ActionName.Should().Be(nameof(PaymentsController.ProcessPayment));
        createdResult.RouteValues.Should().ContainKey("id").WhoseValue.Should().Be(paymentId);
        createdResult.Value.Should().Be(command);

        _mediatorMock.Verify(m => m.Send(It.Is<ProcessPaymentCommand>(c => c == command), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task RefundPayment_ShouldSendCommandAndReturnNoContent()
    {
        // Arrange
        var paymentId = Guid.NewGuid();

        // Act
        var result = await _controller.RefundPayment(paymentId);

        // Assert
        result.Should().BeOfType<NoContentResult>();
        _mediatorMock.Verify(m => m.Send(It.Is<RefundPaymentCommand>(c => c.PaymentId == paymentId), It.IsAny<CancellationToken>()), Times.Once);
    }
}
