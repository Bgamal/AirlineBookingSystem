using AirlineBookingSystem.Payments.Application.Commands;
using AirlineBookingSystem.Payments.Core.Entities;
using AirlineBookingSystem.Payments.Core.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace AirlineBookingSystem.Payments.Application.Handlers
{
    public class ProcessPaymentHandler : IRequestHandler<ProcessPaymentCommand, Guid>
    {
        private readonly IPaymentRepositry _paymentRepository;
        public ProcessPaymentHandler(IPaymentRepositry paymentRepositry)
        {
                _paymentRepository = paymentRepositry;
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
            return payment.Id;
        }
    }
}
