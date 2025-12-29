using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace AirlineBookingSystem.Fights.Core.Entities
{
    public class Flight
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public Guid Id { get; set; }
        [BsonElement("flightNumber")]
        public string FlightNumber { get; set; }
        [BsonElement("origin")]
        public string Origin { get; set; }
        [BsonElement("destination")]
        public string Destination { get; set; }
        [BsonElement("departureTime")]
        [BsonRepresentation(BsonType.DateTime)]
        public DateTime DepartureTime { get; set; }
        [BsonElement("arrivalTime")]
        [BsonRepresentation(BsonType.DateTime)]
        public DateTime ArrivalTime { get; set; }


    }
}
