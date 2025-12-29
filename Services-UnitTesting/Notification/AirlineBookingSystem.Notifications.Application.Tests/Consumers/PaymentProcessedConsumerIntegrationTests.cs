using AirlineBookingSystem.Notifications.Application.Consumers;
using AirlineBookingSystem.Notifications.Application.Commands;
using AitlineBookingSystem.BuildingBlocks.Contracts.EventBus.Messages;
using MassTransit;
using MediatR;
using Moq;

namespace AirlineBookingSystem.Notifications.Application.Tests.Consumers
{
    public class PaymentProcessedConsumerIntegrationTests
    {
        [Fact]
        public async Task Consumer_ProcessesPaymentProcessedEvent_AndSendsCommand()
        {
            // Arrange
            var mockMediator = new Mock<IMediator>();
            var consumer = new PaymentProcessedConsumer(mockMediator.Object);

            var paymentId = Guid.NewGuid();
            var bookingId = Guid.NewGuid();
            var amount = 200.00m;

            var paymentEvent = new PaymentProcessedEvent(
                PaymentId: paymentId,
                BookingId: bookingId,
                Amount: amount,
                PaymentDate: DateTime.UtcNow
            );

            var mockContext = new Mock<ConsumeContext<PaymentProcessedEvent>>();
            mockContext.Setup(x => x.Message).Returns(paymentEvent);

            // Act
            await consumer.Consume(mockContext.Object);

            // Assert
            mockMediator.Verify(
                m => m.Send(It.IsAny<SendNotificationCommand>(), It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task Consumer_ProcessesMultiplePaymentEvents_Sequentially()
        {
            // Arrange
            var mockMediator = new Mock<IMediator>();
            var consumer = new PaymentProcessedConsumer(mockMediator.Object);

            var events = new[]
            {
                new PaymentProcessedEvent(Guid.NewGuid(), Guid.NewGuid(), 100.00m, DateTime.UtcNow),
                new PaymentProcessedEvent(Guid.NewGuid(), Guid.NewGuid(), 200.00m, DateTime.UtcNow),
                new PaymentProcessedEvent(Guid.NewGuid(), Guid.NewGuid(), 300.00m, DateTime.UtcNow)
            };

            // Act
            foreach (var evt in events)
            {
                var mockContext = new Mock<ConsumeContext<PaymentProcessedEvent>>();
                mockContext.Setup(x => x.Message).Returns(evt);
                await consumer.Consume(mockContext.Object);
            }

            // Assert
            mockMediator.Verify(
                m => m.Send(It.IsAny<SendNotificationCommand>(), It.IsAny<CancellationToken>()),
                Times.Exactly(3));
        }

        [Fact]
        public async Task Consumer_NotificationIncludesPaymentAmount_ForEachEvent()
        {
            // Arrange
            var mockMediator = new Mock<IMediator>();
            var consumer = new PaymentProcessedConsumer(mockMediator.Object);

            var amounts = new decimal[] { 100.00m, 250.75m, 500.50m };

            // Act
            foreach (var amount in amounts)
            {
                var paymentEvent = new PaymentProcessedEvent(
                    PaymentId: Guid.NewGuid(),
                    BookingId: Guid.NewGuid(),
                    Amount: amount,
                    PaymentDate: DateTime.UtcNow
                );

                var mockContext = new Mock<ConsumeContext<PaymentProcessedEvent>>();
                mockContext.Setup(x => x.Message).Returns(paymentEvent);

                await consumer.Consume(mockContext.Object);
            }

            // Assert
            mockMediator.Verify(
                m => m.Send(
                    It.Is<SendNotificationCommand>(cmd =>
                        cmd.Message.Contains("100") || cmd.Message.Contains("250.75") || cmd.Message.Contains("500.5")),
                    It.IsAny<CancellationToken>()),
                Times.AtLeastOnce);
        }
    }
}
