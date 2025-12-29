using AirlineBookingSystem.Payments.Application.Commands;
using AirlineBookingSystem.Payments.Application.Consumers;
using AitlineBookingSystem.BuildingBlocks.Contracts.EventBus.Messages;
using MassTransit;
using MediatR;
using Moq;

namespace AirlineBookingSystem.Payments.Application.Tests.Consumers
{
    public class FlightBookedConsumerIntegrationTests
    {
        [Fact]
        public async Task Consumer_ProcessesFlightBookedEvent_AndSendsCommand()
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
                m => m.Send(It.IsAny<ProcessPaymentCommand>(), It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task Consumer_ProcessesMultipleFlightBookedEvents()
        {
            // Arrange
            var mockMediator = new Mock<IMediator>();
            var consumer = new FlightBookedConsumer(mockMediator.Object);

            var bookingIds = new[] { Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid() };

            // Act
            foreach (var bookingId in bookingIds)
            {
                var flightBookedEvent = new FlightBookedEvent(
                    BookingId: bookingId,
                    FlightId: Guid.NewGuid(),
                    PassengerName: "Test User",
                    SeatNumber: "10A",
                    BookingDate: DateTime.UtcNow
                );

                var mockContext = new Mock<ConsumeContext<FlightBookedEvent>>();
                mockContext.Setup(x => x.Message).Returns(flightBookedEvent);

                await consumer.Consume(mockContext.Object);
            }

            // Assert
            mockMediator.Verify(
                m => m.Send(It.IsAny<ProcessPaymentCommand>(), It.IsAny<CancellationToken>()),
                Times.Exactly(3));
        }

        [Fact]
        public async Task Consumer_SendsCorrectPaymentCommandForEachBooking()
        {
            // Arrange
            var mockMediator = new Mock<IMediator>();
            var consumer = new FlightBookedConsumer(mockMediator.Object);

            var bookingId = Guid.NewGuid();
            var flightBookedEvent = new FlightBookedEvent(
                BookingId: bookingId,
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
                    It.Is<ProcessPaymentCommand>(cmd =>
                        cmd.BookingId == bookingId &&
                        cmd.Amount == 200.00m),
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }
    }
}
