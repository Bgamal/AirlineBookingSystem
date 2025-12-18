using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace AirlineBookingSystem.Fights.Application.Commands
{
    public record CreateFlightCommand(string FlightNumber, string Origin, string Destination, DateTime DepartureTime, DateTime ArrivalTime) : IRequest<Guid>;
    
}
