using AirlineBookingSystem.Bookings.Core.Entities;
using AirlineBookingSystem.Bookings.Infrastructure.Repositories;
using Dapper;
using FluentAssertions;
using Microsoft.Data.Sqlite;
using Xunit;

namespace AirlineBookingSystem.Bookings.Infrastructure.Tests.Repositories;

public class BookingRepositoryTests
{
    static BookingRepositoryTests()
    {
        SqlMapper.AddTypeHandler(new SqliteGuidTypeHandler());
    }

    [Fact]
    public async Task AddBookingAsync_ShouldPersistBooking()
    {
        // Arrange
        await using var connection = CreateOpenConnection();
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
        var stored = await connection.QuerySingleOrDefaultAsync<Booking>(
            "SELECT * FROM Bookings WHERE Id = @Id",
            new { booking.Id });

        stored.Should().NotBeNull();
        stored!.Should().BeEquivalentTo(booking, opts => opts
            .Using<DateTime>(ctx => ctx.Subject.Should().BeCloseTo(ctx.Expectation, TimeSpan.FromSeconds(1)))
            .WhenTypeIs<DateTime>());
    }

    [Fact]
    public async Task GetBookingByIdAsync_WhenExists_ShouldReturnBooking()
    {
        // Arrange
        await using var connection = CreateOpenConnection();
        var repository = new BookingRepository(connection);

        var booking = new Booking
        {
            Id = Guid.NewGuid(),
            FlightId = Guid.NewGuid(),
            PassengerName = "Diana Prince",
            SeatNumber = "7B",
            BookingDate = DateTime.UtcNow
        };

        await connection.ExecuteAsync(
            "INSERT INTO Bookings (Id, FlightId, PassengerName, BookingDate, SeatNumber) VALUES (@Id, @FlightId, @PassengerName, @BookingDate, @SeatNumber)",
            booking);

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
        await using var connection = CreateOpenConnection();
        var repository = new BookingRepository(connection);

        // Act
        var result = await repository.GetBookingByIdAsync(Guid.NewGuid());

        // Assert
        result.Should().BeNull();
    }

    private static SqliteConnection CreateOpenConnection()
    {
        var connection = new SqliteConnection("Data Source=:memory:;Mode=Memory;Cache=Shared");
        connection.Open();

        connection.Execute(@"CREATE TABLE IF NOT EXISTS Bookings (
            Id TEXT PRIMARY KEY,
            FlightId TEXT NOT NULL,
            PassengerName TEXT NOT NULL,
            BookingDate TEXT NOT NULL,
            SeatNumber TEXT NOT NULL
        )");

        return connection;
    }
}
