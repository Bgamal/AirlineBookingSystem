using AirlineBookingSystem.Fights.Application.Queries;
using AirlineBookingSystem.Fights.Core.Entities;
using AirlineBookingSystem.Fights.Core.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace AirlineBookingSystem.Fights.Application.Handlers
{
    public class GetAllFlightsHandler : IRequestHandler<GatAllFlightsQuery, IEnumerable<Flight>>
    {
        private readonly IFlightRepository _flightRepository;
        public GetAllFlightsHandler(IFlightRepository flightRepository)
        {
            _flightRepository = flightRepository;
        }
        public async Task<IEnumerable<Flight>> Handle(GatAllFlightsQuery request, CancellationToken cancellationToken)
        {
          return await  _flightRepository.GetAllFlightsAsync();
            //throw new NotImplementedException();
        }
    }
}
