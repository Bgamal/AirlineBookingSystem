using AirlineBookingSystem.Payments.Application.Commands;
using AirlineBookingSystem.Payments.Application.Handlers;
using AirlineBookingSystem.Payments.Core.Entities;
using AirlineBookingSystem.Payments.Core.Repositories;
using FluentAssertions;
using MassTransit;
using Moq;
using Xunit;

namespace AirlineBookingSystem.Payments.Application.Tests.Handlers;

public class ProcessPaymentHandlerTests
{
    private readonly Mock<IPaymentRepositry> _repositoryMock = new();
    private readonly Mock<IPublishEndpoint> _publishEndpointMock = new();
    private readonly ProcessPaymentHandler _handler;

    public ProcessPaymentHandlerTests()
    {
        _publishEndpointMock
            .Setup(p => p.Publish(It.IsAny<object>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _handler = new ProcessPaymentHandler(_repositoryMock.Object, _publishEndpointMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldPersistPaymentAndReturnGeneratedId()
    {
        // Arrange
        Payment? capturedPayment = null;
        _repositoryMock
            .Setup(r => r.ProcessPaymentAsync(It.IsAny<Payment>()))
            .Callback<Payment>(p => capturedPayment = p)
            .Returns(Task.CompletedTask);

        var command = new ProcessPaymentCommand(Guid.NewGuid(), 250m);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeEmpty();
        capturedPayment.Should().NotBeNull();
        capturedPayment!.Id.Should().Be(result);
        capturedPayment.BookingId.Should().Be(command.BookingId);
        capturedPayment.Amount.Should().Be(command.Amount);
        capturedPayment.PaymentDate.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(2));

        _repositoryMock.Verify(r => r.ProcessPaymentAsync(It.IsAny<Payment>()), Times.Once);
    }
}
