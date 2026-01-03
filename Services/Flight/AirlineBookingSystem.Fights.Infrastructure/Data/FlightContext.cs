using AirlineBookingSystem.Fights.Core.Entities;
using Microsoft.IdentityModel.Protocols;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Configuration;

namespace AirlineBookingSystem.Fights.Infrastructure.Data
{
    public class FlightContext : IFlightContext
    {
        public IMongoCollection<Flight> Flights { get; }
        public FlightContext(IConfiguration configuration)
        {
            var mongoClient = new MongoClient(configuration["DatabaseSettings:ConnectionString"]);
            var mongoDatabase = mongoClient.GetDatabase(configuration["DatabaseSettings:DatabaseName"]);
            Flights = mongoDatabase.GetCollection<Flight>(configuration["DatabaseSettings:CollectionName"]);

        }
    }
}
