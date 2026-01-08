using AirlineBookingSystem.Payments.Application.Commands;
using AirlineBookingSystem.Payments.Application.Handlers;
using AirlineBookingSystem.Payments.Core.Repositories;
using Moq;
using Xunit;

namespace AirlineBookingSystem.Payments.Application.Tests.Handlers;

public class RefundPaymentHandlerTests
{
    private readonly Mock<IPaymentRepositry> _repositoryMock = new();
    private readonly RefundPaymentHandler _handler;

    public RefundPaymentHandlerTests()
    {
        _handler = new RefundPaymentHandler(_repositoryMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldInvokeRepositoryRefund()
    {
        // Arrange
        var paymentId = Guid.NewGuid();
        var command = new RefundPaymentCommand(paymentId);

        _repositoryMock
            .Setup(r => r.RefundPaymentAsync(paymentId))
            .ReturnsAsync(new AirlineBookingSystem.Payments.Core.Entities.Payment())
            .Verifiable();

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _repositoryMock.Verify(r => r.RefundPaymentAsync(paymentId), Times.Once);
    }
}
