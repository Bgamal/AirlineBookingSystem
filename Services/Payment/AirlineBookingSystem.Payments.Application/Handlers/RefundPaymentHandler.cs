using AirlineBookingSystem.Payments.Application.Commands;
using AirlineBookingSystem.Payments.Core.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AirlineBookingSystem.Payments.Application.Handlers
{
    public class RefundPaymentHandler : IRequestHandler<RefundPaymentCommand>
    {
        private readonly IPaymentRepositry _paymentRepository;
        public RefundPaymentHandler(IPaymentRepositry paymentRepositry)
        {
            _paymentRepository = paymentRepositry;
        }

        public async Task Handle(RefundPaymentCommand request, CancellationToken cancellationToken)
        {
            await _paymentRepository.RefundPaymentAsync(request.PaymentId);
        }
    }
}
