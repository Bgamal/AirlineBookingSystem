using AirlineBookingSystem.Payments.Application.Commands;
using AirlineBookingSystem.Payments.Application.Consumers;
using AitlineBookingSystem.BuildingBlocks.Contracts.EventBus.Messages;
using FluentAssertions;
using MassTransit;
using MediatR;
using Moq;

namespace AirlineBookingSystem.Payments.Application.Tests.Consumers
{
    public class FlightBookedConsumerTests
    {
        [Fact]
        public async Task Consume_WithValidFlightBookedEvent_SendsProcessPaymentCommand()
        {
            // Arrange
            var mockMediator = new Mock<IMediator>();
            var consumer = new FlightBookedConsumer(mockMediator.Object);

            var bookingId = Guid.NewGuid();
            var flightBookedEvent = new FlightBookedEvent(
                BookingId: bookingId,
                FlightId: Guid.NewGuid(),
                PassengerName: "John Doe",
                SeatNumber: "12A",
                BookingDate: DateTime.UtcNow
            );

            var mockContext = new Mock<ConsumeContext<FlightBookedEvent>>();
            mockContext.Setup(x => x.Message).Returns(flightBookedEvent);

            // Act
            await consumer.Consume(mockContext.Object);

            // Assert
            mockMediator.Verify(
                m => m.Send(
                    It.Is<ProcessPaymentCommand>(cmd =>
                        cmd.BookingId == bookingId &&
                        cmd.Amount == 200.00m),
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public void Constructor_WithNullMediator_ThrowsArgumentNullException()
        {
            // Act & Assert
            var exception = Assert.Throws<ArgumentNullException>(() => new FlightBookedConsumer(null!));
            exception.ParamName.Should().Be("mediator");
        }

        [Fact]
        public async Task Consume_CommandHasCorrectBookingId()
        {
            // Arrange
            var mockMediator = new Mock<IMediator>();
            var consumer = new FlightBookedConsumer(mockMediator.Object);

            var expectedBookingId = Guid.NewGuid();
            var flightBookedEvent = new FlightBookedEvent(
                BookingId: expectedBookingId,
                FlightId: Guid.NewGuid(),
                PassengerName: "Jane Smith",
                SeatNumber: "14B",
                BookingDate: DateTime.UtcNow
            );

            var mockContext = new Mock<ConsumeContext<FlightBookedEvent>>();
            mockContext.Setup(x => x.Message).Returns(flightBookedEvent);

            // Act
            await consumer.Consume(mockContext.Object);

            // Assert
            mockMediator.Verify(
                m => m.Send(
                    It.Is<ProcessPaymentCommand>(cmd => cmd.BookingId == expectedBookingId),
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task Consume_DefaultPaymentAmountIs200()
        {
            // Arrange
            var mockMediator = new Mock<IMediator>();
            var consumer = new FlightBookedConsumer(mockMediator.Object);

            var flightBookedEvent = new FlightBookedEvent(
                BookingId: Guid.NewGuid(),
                FlightId: Guid.NewGuid(),
                PassengerName: "Test User",
                SeatNumber: "10C",
                BookingDate: DateTime.UtcNow
            );

            var mockContext = new Mock<ConsumeContext<FlightBookedEvent>>();
            mockContext.Setup(x => x.Message).Returns(flightBookedEvent);

            // Act
            await consumer.Consume(mockContext.Object);

            // Assert
            mockMediator.Verify(
                m => m.Send(
                    It.Is<ProcessPaymentCommand>(cmd => cmd.Amount == 200.00m),
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }
    }
}
