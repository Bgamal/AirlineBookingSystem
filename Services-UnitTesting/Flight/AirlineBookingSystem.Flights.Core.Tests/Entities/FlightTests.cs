using AirlineBookingSystem.Fights.Core.Entities;
using FluentAssertions;

namespace AirlineBookingSystem.Flights.Core.Tests.Entities;

public class FlightTests
{
    [Fact]
    public void Constructor_ShouldAllowPropertyInitialization()
    {
        // Arrange
        var id = Guid.NewGuid();
        var departure = new DateTime(2025, 1, 1, 8, 0, 0, DateTimeKind.Utc);
        var arrival = departure.AddHours(3);

        // Act
        var flight = new Flight
        {
            Id = id,
            FlightNumber = "AB123",
            Origin = "Cairo",
            Destination = "Riyadh",
            DepartureTime = departure,
            ArrivalTime = arrival
        };

        // Assert
        flight.Id.Should().Be(id);
        flight.FlightNumber.Should().Be("AB123");
        flight.Origin.Should().Be("Cairo");
        flight.Destination.Should().Be("Riyadh");
        flight.DepartureTime.Should().Be(departure);
        flight.ArrivalTime.Should().Be(arrival);
    }
}
