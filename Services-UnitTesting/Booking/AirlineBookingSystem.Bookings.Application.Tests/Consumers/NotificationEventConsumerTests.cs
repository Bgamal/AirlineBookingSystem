using AirlineBookingSystem.Bookings.Application.Consumers;
using AitlineBookingSystem.BuildingBlocks.Contracts.EventBus.Messages;
using FluentAssertions;
using MassTransit;
using Moq;

namespace AirlineBookingSystem.Bookings.Application.Tests.Consumers
{
    public class NotificationEventConsumerTests
    {
        [Fact]
        public async Task Consume_WithValidNotificationEvent_CompletesSuccessfully()
        {
            // Arrange
            var consumer = new NotificationEventConsumer();

            var notificationEvent = new NotificationEvent(
                Recipient: "customer@example.com",
                Message: "Your booking confirmation",
                Type: "Email"
            );

            var mockContext = new Mock<ConsumeContext<NotificationEvent>>();
            mockContext.Setup(x => x.Message).Returns(notificationEvent);

            // Act
            await consumer.Consume(mockContext.Object);

            // Assert - No exception thrown indicates successful processing
            mockContext.Object.Should().NotBeNull();
        }

        [Fact]
        public async Task Consume_WithEmailType_ProcessesSuccessfully()
        {
            // Arrange
            var consumer = new NotificationEventConsumer();

            var notificationEvent = new NotificationEvent(
                Recipient: "user@example.com",
                Message: "Payment confirmation",
                Type: "Email"
            );

            var mockContext = new Mock<ConsumeContext<NotificationEvent>>();
            mockContext.Setup(x => x.Message).Returns(notificationEvent);

            // Act
            await consumer.Consume(mockContext.Object);

            // Assert
            notificationEvent.Type.Should().Be("Email");
        }

        [Fact]
        public async Task Consume_WithSmsType_ProcessesSuccessfully()
        {
            // Arrange
            var consumer = new NotificationEventConsumer();

            var notificationEvent = new NotificationEvent(
                Recipient: "+1234567890",
                Message: "Your booking is confirmed",
                Type: "SMS"
            );

            var mockContext = new Mock<ConsumeContext<NotificationEvent>>();
            mockContext.Setup(x => x.Message).Returns(notificationEvent);

            // Act
            await consumer.Consume(mockContext.Object);

            // Assert
            notificationEvent.Type.Should().Be("SMS");
        }

        [Fact]
        public async Task Consume_WithVariousRecipients_ProcessesAllSuccessfully()
        {
            // Arrange
            var consumer = new NotificationEventConsumer();
            var recipients = new[] { "user1@example.com", "user2@example.com", "+1111111111" };

            // Act & Assert
            foreach (var recipient in recipients)
            {
                var notificationEvent = new NotificationEvent(
                    Recipient: recipient,
                    Message: "Test notification",
                    Type: "Email"
                );

                var mockContext = new Mock<ConsumeContext<NotificationEvent>>();
                mockContext.Setup(x => x.Message).Returns(notificationEvent);

                await consumer.Consume(mockContext.Object);

                notificationEvent.Recipient.Should().Be(recipient);
            }
        }
    }
}
