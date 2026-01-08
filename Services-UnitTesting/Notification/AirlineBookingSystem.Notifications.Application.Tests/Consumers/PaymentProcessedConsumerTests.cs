using AirlineBookingSystem.Notifications.Application.Commands;
using AirlineBookingSystem.Notifications.Application.Consumers;
using AitlineBookingSystem.BuildingBlocks.Contracts.EventBus.Messages;
using FluentAssertions;
using MassTransit;
using MediatR;
using Moq;

namespace AirlineBookingSystem.Notifications.Application.Tests.Consumers
{
    public class PaymentProcessedConsumerTests
    {
        [Fact]
        public async Task Consume_WithValidPaymentProcessedEvent_SendsNotificationCommand()
        {
            // Arrange
            var mockMediator = new Mock<IMediator>();
            var consumer = new PaymentProcessedConsumer(mockMediator.Object);
            
            var paymentEvent = new PaymentProcessedEvent(
                PaymentId: Guid.NewGuid(),
                BookingId: Guid.NewGuid(),
                Amount: 150.00m,
                PaymentDate: DateTime.UtcNow
            );

            var mockContext = new Mock<ConsumeContext<PaymentProcessedEvent>>();
            mockContext.Setup(x => x.Message).Returns(paymentEvent);

            // Act
            await consumer.Consume(mockContext.Object);

            // Assert
            mockMediator.Verify(
                m => m.Send(
                    It.Is<SendNotificationCommand>(cmd =>
                        cmd.Recipient == "test@example.com" &&
                        cmd.Type == "Email" &&
                        cmd.Message.Contains("Payment") &&
                        cmd.Message.Contains(paymentEvent.Amount.ToString())),
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public void Constructor_WithNullMediator_ThrowsArgumentNullException()
        {
            // Act & Assert
            var exception = Assert.Throws<ArgumentNullException>(() => new PaymentProcessedConsumer(null!));
            exception.ParamName.Should().Be("mediator");
        }

        [Fact]
        public async Task Consume_NotificationMessageContainsCorrectDetails()
        {
            // Arrange
            var mockMediator = new Mock<IMediator>();
            var consumer = new PaymentProcessedConsumer(mockMediator.Object);

            var bookingId = Guid.NewGuid();
            var amount = 250.50m;
            var paymentDate = DateTime.UtcNow;
            
            var paymentEvent = new PaymentProcessedEvent(
                PaymentId: Guid.NewGuid(),
                BookingId: bookingId,
                Amount: amount,
                PaymentDate: paymentDate
            );

            var mockContext = new Mock<ConsumeContext<PaymentProcessedEvent>>();
            mockContext.Setup(x => x.Message).Returns(paymentEvent);

            // Act
            await consumer.Consume(mockContext.Object);

            // Assert
            mockMediator.Verify(
                m => m.Send(
                    It.Is<SendNotificationCommand>(cmd =>
                        cmd.Message.Contains(amount.ToString()) &&
                        cmd.Message.Contains(bookingId.ToString()) &&
                        cmd.Message.Contains("successfully")),
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }
    }
}
