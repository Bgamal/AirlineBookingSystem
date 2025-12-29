using System;
using System.Collections.Generic;
using System.Text;
using AirlineBookingSystem.Notifications.Application.Commands;
using AitlineBookingSystem.BuildingBlocks.Contracts.EventBus.Messages;
using MassTransit;
using MediatR;

namespace AirlineBookingSystem.Notifications.Application.Consumers
{
    public class PaymentProcessedConsumer : IConsumer<PaymentProcessedEvent>
    {
        private readonly IMediator _mediator;
        public PaymentProcessedConsumer(IMediator mediator)
        {
                _mediator = mediator;
        }
        public async Task Consume(ConsumeContext<PaymentProcessedEvent> context)
        {
            var paymentProcessedEvent = context.Message;
            // Handle the payment processed event, e.g., send notification 
            var message = $"Payment of {paymentProcessedEvent.Amount} for Booking ID {paymentProcessedEvent.BookingId} has been processed successfully on {paymentProcessedEvent.PaymentDate}.";
            var command = new SendNotificationCommand
           (
               "test@example.com", // Assuming BookingId is used as UserId for notification
                 message,"Email"
            );
            await _mediator.Send(command);
        }
    }
}
