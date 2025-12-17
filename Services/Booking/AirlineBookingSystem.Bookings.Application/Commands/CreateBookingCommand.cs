using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace AirlineBookingSystem.Bookings.Application.Commands
{
    public record CreateBookingCommand(Guid FlightId, string PassengerName, string SeatNumber): IRequest<Guid> ;
   
}
