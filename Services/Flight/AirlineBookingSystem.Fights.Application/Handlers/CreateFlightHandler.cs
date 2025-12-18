using AirlineBookingSystem.Fights.Application.Commands;
using AirlineBookingSystem.Fights.Core.Entities;
using AirlineBookingSystem.Fights.Core.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace AirlineBookingSystem.Fights.Application.Handlers
{
    public class CreateFlightHandler:IRequestHandler<CreateFlightCommand,Guid>
    {
        private readonly IFlightRepository _flightRepository;
        public CreateFlightHandler(IFlightRepository flightRepository)
        {
            _flightRepository= flightRepository;
        }

        public async Task<Guid> Handle(CreateFlightCommand request, CancellationToken cancellationToken)
        {
            var flight =new Flight
            {
                Id=new Guid(),
                FlightNumber = request.FlightNumber,
                Origin = request.Origin,
                Destination = request.Destination,
                DepartureTime = request.DepartureTime,
                ArrivalTime = request.ArrivalTime
            };
             await _flightRepository.AddFlightAsync(flight);
            return flight.Id;
            //  throw new NotImplementedException();
        }
    }
}
