using AirlineBookingSystem.Fights.Core.Entities;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Text;

namespace AirlineBookingSystem.Fights.Infrastructure.Data
{
    public interface IFlightContext
    {
        IMongoCollection<Flight> Flights { get; }


    }
}
