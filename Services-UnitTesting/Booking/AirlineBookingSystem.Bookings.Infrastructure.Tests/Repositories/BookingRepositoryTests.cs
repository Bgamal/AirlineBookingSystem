using AirlineBookingSystem.Bookings.Core.Entities;
using AirlineBookingSystem.Bookings.Infrastructure.Repositories;
using FluentAssertions;
using Newtonsoft.Json;
using StackExchange.Redis;
using Xunit;

namespace AirlineBookingSystem.Bookings.Infrastructure.Tests.Repositories;

public class BookingRepositoryTests
{
    private const string RedisKeyPrefix = "booking_";

    [Fact]
    public async Task AddBookingAsync_ShouldPersistBooking()
    {
        // Arrange
        using var connection = ConnectionMultiplexer.Connect("127.0.0.1:6379");
        var database = connection.GetDatabase();
        await database.ExecuteAsync("FLUSHDB");

        var repository = new BookingRepository(connection);
        var booking = new Booking
        {
            Id = Guid.NewGuid(),
            FlightId = Guid.NewGuid(),
            PassengerName = "Bruce Wayne",
            SeatNumber = "1A",
            BookingDate = DateTime.UtcNow
        };

        // Act
        await repository.AddBookingAsync(booking);

        // Assert
        var stored = await database.StringGetAsync($"{RedisKeyPrefix}{booking.Id}");
        stored.HasValue.Should().BeTrue();

        var deserialized = JsonConvert.DeserializeObject<Booking>(stored.ToString());
        deserialized.Should().NotBeNull();
        deserialized!.Should().BeEquivalentTo(booking, opts => opts
            .Using<DateTime>(ctx => ctx.Subject.Should().BeCloseTo(ctx.Expectation, TimeSpan.FromSeconds(1)))
            .WhenTypeIs<DateTime>());
    }

    [Fact]
    public async Task GetBookingByIdAsync_WhenExists_ShouldReturnBooking()
    {
        // Arrange
        using var connection = ConnectionMultiplexer.Connect("127.0.0.1:6379");
        var database = connection.GetDatabase();
        await database.ExecuteAsync("FLUSHDB");

        var repository = new BookingRepository(connection);

        var booking = new Booking
        {
            Id = Guid.NewGuid(),
            FlightId = Guid.NewGuid(),
            PassengerName = "Diana Prince",
            SeatNumber = "7B",
            BookingDate = DateTime.UtcNow
        };

        var data = JsonConvert.SerializeObject(booking);
        await database.StringSetAsync($"{RedisKeyPrefix}{booking.Id}", data);

        // Act
        var result = await repository.GetBookingByIdAsync(booking.Id);

        // Assert
        result.Should().NotBeNull();
        result!.Should().BeEquivalentTo(booking, opts => opts
            .Using<DateTime>(ctx => ctx.Subject.Should().BeCloseTo(ctx.Expectation, TimeSpan.FromSeconds(1)))
            .WhenTypeIs<DateTime>());
    }

    [Fact]
    public async Task GetBookingByIdAsync_WhenMissing_ShouldReturnNull()
    {
        // Arrange
        using var connection = ConnectionMultiplexer.Connect("127.0.0.1:6379");
        var database = connection.GetDatabase();
        await database.ExecuteAsync("FLUSHDB");

        var repository = new BookingRepository(connection);

        // Act
        var result = await repository.GetBookingByIdAsync(Guid.NewGuid());

        // Assert
        result.Should().BeNull();
    }
}
