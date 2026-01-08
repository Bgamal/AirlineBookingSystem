using AirlineBookingSystem.Payments.Application.Commands;
using AirlineBookingSystem.Payments.Application.Handlers;
using AirlineBookingSystem.Payments.Core.Entities;
using AirlineBookingSystem.Payments.Core.Repositories;
using AitlineBookingSystem.BuildingBlocks.Contracts.EventBus.Messages;
using FluentAssertions;
using MassTransit;
using Moq;

namespace AirlineBookingSystem.Payments.Application.Tests.Handlers
{
    public class ProcessPaymentHandlerPublisherTests
    {
        [Fact]
        public async Task Handle_WithValidCommand_PublishesPaymentProcessedEvent()
        {
            // Arrange
            var mockPaymentRepository = new Mock<IPaymentRepositry>();
            var mockPublishEndpoint = new Mock<IPublishEndpoint>();

            var handler = new ProcessPaymentHandler(mockPaymentRepository.Object, mockPublishEndpoint.Object);

            var bookingId = Guid.NewGuid();
            var command = new ProcessPaymentCommand(bookingId, 200.00m);

            mockPaymentRepository
                .Setup(r => r.ProcessPaymentAsync(It.IsAny<Payment>()))
                .Returns(Task.CompletedTask);

            mockPublishEndpoint
                .Setup(p => p.Publish(It.IsAny<PaymentProcessedEvent>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            var paymentId = await handler.Handle(command, CancellationToken.None);

            // Assert
            paymentId.Should().NotBe(Guid.Empty);
            mockPaymentRepository.Verify(
                r => r.ProcessPaymentAsync(It.Is<Payment>(p =>
                    p.BookingId == bookingId &&
                    p.Amount == 200.00m)),
                Times.Once);
            mockPublishEndpoint.Verify(
                p => p.Publish(It.IsAny<PaymentProcessedEvent>(), It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task Handle_PublishedEventContainsCorrectPaymentDetails()
        {
            // Arrange
            var mockPaymentRepository = new Mock<IPaymentRepositry>();
            var mockPublishEndpoint = new Mock<IPublishEndpoint>();

            var handler = new ProcessPaymentHandler(mockPaymentRepository.Object, mockPublishEndpoint.Object);

            var bookingId = Guid.NewGuid();
            var amount = 350.75m;
            var command = new ProcessPaymentCommand(bookingId, amount);

            mockPaymentRepository
                .Setup(r => r.ProcessPaymentAsync(It.IsAny<Payment>()))
                .Returns(Task.CompletedTask);

            mockPublishEndpoint
                .Setup(p => p.Publish(It.IsAny<PaymentProcessedEvent>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            await handler.Handle(command, CancellationToken.None);

            // Assert
            mockPaymentRepository.Verify(
                r => r.ProcessPaymentAsync(It.Is<Payment>(p =>
                    p.BookingId == bookingId &&
                    p.Amount == amount &&
                    p.PaymentDate != DateTime.MinValue)),
                Times.Once);
        }

        [Fact]
        public void Constructor_WithNullRepository_ThrowsArgumentNullException()
        {
            // Act & Assert
            var exception = Assert.Throws<ArgumentNullException>(
                () => new ProcessPaymentHandler(null!, It.IsAny<IPublishEndpoint>()));
            exception.ParamName.Should().Be("paymentRepositry");
        }

        [Fact]
        public void Constructor_WithNullPublishEndpoint_ThrowsArgumentNullException()
        {
            // Arrange
            var mockRepository = new Mock<IPaymentRepositry>();

            // Act & Assert
            var exception = Assert.Throws<ArgumentNullException>(
                () => new ProcessPaymentHandler(mockRepository.Object, null!));
            exception.ParamName.Should().Be("publishEndpoint");
        }

        [Fact]
        public async Task Handle_SavesPaymentToRepository()
        {
            // Arrange
            var mockPaymentRepository = new Mock<IPaymentRepositry>();
            var mockPublishEndpoint = new Mock<IPublishEndpoint>();

            var handler = new ProcessPaymentHandler(mockPaymentRepository.Object, mockPublishEndpoint.Object);

            var command = new ProcessPaymentCommand(Guid.NewGuid(), 150.00m);

            mockPaymentRepository
                .Setup(r => r.ProcessPaymentAsync(It.IsAny<Payment>()))
                .Returns(Task.CompletedTask);

            mockPublishEndpoint
                .Setup(p => p.Publish(It.IsAny<PaymentProcessedEvent>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            await handler.Handle(command, CancellationToken.None);

            // Assert
            mockPaymentRepository.Verify(
                r => r.ProcessPaymentAsync(It.IsAny<Payment>()),
                Times.Once);
        }

        [Fact]
        public async Task Handle_ReturnsValidPaymentId()
        {
            // Arrange
            var mockPaymentRepository = new Mock<IPaymentRepositry>();
            var mockPublishEndpoint = new Mock<IPublishEndpoint>();

            var handler = new ProcessPaymentHandler(mockPaymentRepository.Object, mockPublishEndpoint.Object);
            var command = new ProcessPaymentCommand(Guid.NewGuid(), 100.00m);

            mockPaymentRepository
                .Setup(r => r.ProcessPaymentAsync(It.IsAny<Payment>()))
                .Returns(Task.CompletedTask);

            mockPublishEndpoint
                .Setup(p => p.Publish(It.IsAny<PaymentProcessedEvent>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBe(Guid.Empty);
        }
    }
}
