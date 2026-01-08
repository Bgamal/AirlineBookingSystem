using AirlineBookingSystem.Fights.Core.Entities;
using AirlineBookingSystem.Fights.Core.Repositories;
using AirlineBookingSystem.Fights.Infrastructure.Data;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using MongoDB.Driver;
namespace AirlineBookingSystem.Fights.Infrastructure.Repositories
{
    public class FlightRepository : IFlightRepository
    {
       // Dapper
       // private readonly IDbConnection _dbConnection;

        //MonoDB
        private  readonly IFlightContext _flightContext;
        //public FlightRepository(IDbConnection dbConnection)
        //{
        //    _dbConnection = dbConnection;
        //}
        public FlightRepository(IFlightContext flightContext)
        {
         _flightContext = flightContext;
        }
        public async Task AddFlightAsync(Flight flight)
        {
           await _flightContext.Flights.InsertOneAsync(flight);
        }

        public async Task DeleteFlightAsync(Guid flightId)
        {
          await  _flightContext.Flights.DeleteOneAsync(f => f.Id == flightId);
        }

        public async Task<IEnumerable<Flight>> GetAllFlightsAsync()
        {
           return await _flightContext.Flights.Find(_ => true).ToListAsync();
        }

        public async Task<Flight> GetFlightByIdAsync(Guid flightId)
        {
           return await _flightContext.Flights.Find(f => f.Id == flightId).FirstOrDefaultAsync();
        }
    }
}
