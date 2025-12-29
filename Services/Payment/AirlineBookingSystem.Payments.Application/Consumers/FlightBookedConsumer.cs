using AirlineBookingSystem.Payments.Application.Commands;
using AitlineBookingSystem.BuildingBlocks.Contracts.EventBus.Messages;
using MassTransit;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace AirlineBookingSystem.Payments.Application.Consumers
{
    public class FlightBookedConsumer : IConsumer<FlightBookedEvent>
    {
        private readonly IMediator _mediator;
        public FlightBookedConsumer(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }
        public async Task Consume(ConsumeContext<FlightBookedEvent> context)
        {
            var message = context.Message;
            // Handle the FlightBookedEvent message
            var command = new ProcessPaymentCommand
            (
                message.BookingId,
               200.00m// 
            );
            await _mediator.Send(command);
        }
    }
}
