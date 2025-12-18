using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace AirlineBookingSystem.Payments.Application.Commands
{
    public record ProcessPaymentCommand(
        Guid BookingId,
        decimal Amount
       ) : IRequest<Guid>;

}
