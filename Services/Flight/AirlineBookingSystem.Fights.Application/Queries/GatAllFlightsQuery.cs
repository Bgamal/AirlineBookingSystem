using AirlineBookingSystem.Fights.Core.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace AirlineBookingSystem.Fights.Application.Queries
{
    public record GatAllFlightsQuery :IRequest<IEnumerable<Flight>>;
    
}
