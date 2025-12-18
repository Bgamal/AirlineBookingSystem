using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace AirlineBookingSystem.Fights.Application.Commands
{
    public record DeleteFlightCommand(Guid Id):IRequest;
   
}
