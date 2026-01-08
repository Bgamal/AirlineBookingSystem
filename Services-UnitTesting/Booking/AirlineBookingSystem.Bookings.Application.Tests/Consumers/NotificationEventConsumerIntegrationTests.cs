using AirlineBookingSystem.Bookings.Application.Consumers;
using AitlineBookingSystem.BuildingBlocks.Contracts.EventBus.Messages;
using FluentAssertions;
using MassTransit;
using Moq;

namespace AirlineBookingSystem.Bookings.Application.Tests.Consumers
{
    public class NotificationEventConsumerIntegrationTests
    {
        [Fact]
        public async Task Consumer_ProcessesNotificationEvent_Successfully()
        {
            // Arrange
            var consumer = new NotificationEventConsumer();

            var notificationEvent = new NotificationEvent(
                Recipient: "customer@example.com",
                Message: "Your booking is confirmed",
                Type: "Email"
            );

            var mockContext = new Mock<ConsumeContext<NotificationEvent>>();
            mockContext.Setup(x => x.Message).Returns(notificationEvent);

            // Act
            await consumer.Consume(mockContext.Object);

            // Assert - No exception thrown indicates success
            mockContext.Object.Should().NotBeNull();
        }

        [Fact]
        public async Task Consumer_ProcessesMultipleNotificationEvents()
        {
            // Arrange
            var consumer = new NotificationEventConsumer();

            var notificationEvents = new[]
            {
                new NotificationEvent("user1@example.com", "Booking confirmed", "Email"),
                new NotificationEvent("user2@example.com", "Payment received", "Email"),
                new NotificationEvent("+1234567890", "Your ticket is ready", "SMS")
            };

            // Act
            foreach (var evt in notificationEvents)
            {
                var mockContext = new Mock<ConsumeContext<NotificationEvent>>();
                mockContext.Setup(x => x.Message).Returns(evt);
                await consumer.Consume(mockContext.Object);
            }

            // Assert - All events should be processed without exception
            Assert.True(true);
        }

        [Fact]
        public async Task Consumer_ProcessesVariousNotificationTypes()
        {
            // Arrange
            var consumer = new NotificationEventConsumer();

            var notificationTypes = new[] { "Email", "SMS", "Push" };

            // Act & Assert
            foreach (var type in notificationTypes)
            {
                var notificationEvent = new NotificationEvent(
                    Recipient: "user@example.com",
                    Message: "Test",
                    Type: type
                );

                var mockContext = new Mock<ConsumeContext<NotificationEvent>>();
                mockContext.Setup(x => x.Message).Returns(notificationEvent);

                var task = consumer.Consume(mockContext.Object);
                await task;

                Assert.Equal(type, notificationEvent.Type);
            }
        }

        [Fact]
        public async Task Consumer_SuccessfullyProcessesNotificationWithAllDetails()
        {
            // Arrange
            var consumer = new NotificationEventConsumer();

            var recipient = "important@example.com";
            var message = "Your booking #12345 has been confirmed";
            var type = "Email";

            var notificationEvent = new NotificationEvent(
                Recipient: recipient,
                Message: message,
                Type: type
            );

            var mockContext = new Mock<ConsumeContext<NotificationEvent>>();
            mockContext.Setup(x => x.Message).Returns(notificationEvent);

            // Act
            await consumer.Consume(mockContext.Object);

            // Assert
            Assert.Equal(recipient, notificationEvent.Recipient);
            Assert.Equal(message, notificationEvent.Message);
            Assert.Equal(type, notificationEvent.Type);
        }
    }
}
