using AirlineBookingSystem.Payments.Application.Commands;
using AirlineBookingSystem.Payments.Core.Entities;
using AirlineBookingSystem.Payments.Core.Repositories;
using AitlineBookingSystem.BuildingBlocks.Contracts.EventBus.Messages;
using MassTransit;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace AirlineBookingSystem.Payments.Application.Handlers
{
    public class ProcessPaymentHandler : IRequestHandler<ProcessPaymentCommand, Guid>
    {
        private readonly IPaymentRepositry _paymentRepository;
        private readonly IPublishEndpoint _publishEndpoint;
        public ProcessPaymentHandler(IPaymentRepositry paymentRepositry,IPublishEndpoint publishEndpoint)
        {
            _paymentRepository = paymentRepositry ?? throw new ArgumentNullException(nameof(paymentRepositry));
            _publishEndpoint = publishEndpoint ?? throw new ArgumentNullException(nameof(publishEndpoint));
        }

        public async Task<Guid> Handle(ProcessPaymentCommand request, CancellationToken cancellationToken)
        {
            // Implement payment processing logic here
           // Simulate payment processing and generate a payment ID
            // Save payment details to the repository
            var payment = new Payment
            {
                Id = Guid.NewGuid(),
                BookingId = request.BookingId,
                Amount = request.Amount,
                PaymentDate = DateTime.UtcNow
            };
            
            await _paymentRepository.ProcessPaymentAsync(payment);
            // Optionally, publish a PaymentProcessed event
            await _publishEndpoint.Publish(new PaymentProcessedEvent
            (
                payment.Id,
                 payment.BookingId,
                 payment.Amount,
                 payment.PaymentDate
           ));
            return payment.Id;
        }
    }
}
