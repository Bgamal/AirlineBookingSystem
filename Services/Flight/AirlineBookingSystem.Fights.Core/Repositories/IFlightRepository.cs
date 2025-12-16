using AirlineBookingSystem.Fights.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace AirlineBookingSystem.Fights.Core.Repositories
{
    public interface IFlightRepository
    {
        Task<Flight> GetFlightByIdAsync(Guid flightId);
        Task AddFlightAsync(Flight flight);
        Task<IEnumerable<Flight>> GetAllFlightsAsync();
        Task DeleteFlightAsunc(Guid flightId);
    }
}
