using AirlineBookingSystem.Payments.Core.Entities;
using AirlineBookingSystem.Payments.Infrastructure.Repositories;
using Dapper;
using FluentAssertions;
using Microsoft.Data.Sqlite;
using Xunit;

namespace AirlineBookingSystem.Payments.Infrastructure.Tests.Repositories;

public class PaymentRepositoryTests
{
    static PaymentRepositoryTests()
    {
        SqlMapper.AddTypeHandler(new SqliteGuidTypeHandler());
    }

    [Fact]
    public async Task ProcessPaymentAsync_ShouldPersistPayment()
    {
        // Arrange
        await using var connection = CreateOpenConnection();
        var repository = new PaymentRepositry(connection);
        var payment = new Payment
        {
            Id = Guid.NewGuid(),
            BookingId = Guid.NewGuid(),
            Amount = 250m,
            PaymentDate = DateTime.UtcNow
        };

        // Act
        await repository.ProcessPaymentAsync(payment);

        // Assert
        var stored = await connection.QuerySingleOrDefaultAsync<Payment>(
            "SELECT * FROM Payments WHERE Id = @Id",
            new { payment.Id });

        stored.Should().NotBeNull();
        stored!.Should().BeEquivalentTo(payment, opts => opts
            .Using<DateTime>(ctx => ctx.Subject.Should().BeCloseTo(ctx.Expectation, TimeSpan.FromSeconds(1)))
            .WhenTypeIs<DateTime>());
    }

    [Fact]
    public async Task GetPaymentByIdAsync_WhenExists_ShouldReturnPayment()
    {
        // Arrange
        await using var connection = CreateOpenConnection();
        var repository = new PaymentRepositry(connection);

        var payment = new Payment
        {
            Id = Guid.NewGuid(),
            BookingId = Guid.NewGuid(),
            Amount = 300m,
            PaymentDate = DateTime.UtcNow
        };

        await connection.ExecuteAsync(
            "INSERT INTO Payments (Id, BookingId, Amount, PaymentDate) VALUES (@Id, @BookingId, @Amount, @PaymentDate)",
            payment);

        // Act
        var result = await repository.GetPaymentByIdAsync(payment.Id);

        // Assert
        result.Should().NotBeNull();
        result!.Should().BeEquivalentTo(payment, opts => opts
            .Using<DateTime>(ctx => ctx.Subject.Should().BeCloseTo(ctx.Expectation, TimeSpan.FromSeconds(1)))
            .WhenTypeIs<DateTime>());
    }

    [Fact]
    public async Task RefundPaymentAsync_ShouldReturnPayment()
    {
        // Arrange
        await using var connection = CreateOpenConnection();
        var repository = new PaymentRepositry(connection);

        var payment = new Payment
        {
            Id = Guid.NewGuid(),
            BookingId = Guid.NewGuid(),
            Amount = 400m,
            PaymentDate = DateTime.UtcNow
        };

        await connection.ExecuteAsync(
            "INSERT INTO Payments (Id, BookingId, Amount, PaymentDate) VALUES (@Id, @BookingId, @Amount, @PaymentDate)",
            payment);

        // Act
        var result = await repository.RefundPaymentAsync(payment.Id);

        // Assert
        result.Should().NotBeNull();

        var stored = await connection.QuerySingleOrDefaultAsync<Payment>(
            "SELECT * FROM Payments WHERE Id = @Id",
            new { payment.Id });

        stored.Should().NotBeNull();
    }

    [Fact]
    public async Task DeletePaymentAsync_ShouldRemovePayment()
    {
        // Arrange
        await using var connection = CreateOpenConnection();
        var repository = new PaymentRepositry(connection);

        var payment = new Payment
        {
            Id = Guid.NewGuid(),
            BookingId = Guid.NewGuid(),
            Amount = 150m,
            PaymentDate = DateTime.UtcNow
        };

        await connection.ExecuteAsync(
            "INSERT INTO Payments (Id, BookingId, Amount, PaymentDate) VALUES (@Id, @BookingId, @Amount, @PaymentDate)",
            payment);

        // Act
        await repository.DeletePaymentAsync(payment.Id);

        // Assert
        var result = await connection.QuerySingleOrDefaultAsync<Payment>(
            "SELECT * FROM Payments WHERE Id = @Id",
            new { payment.Id });

        result.Should().BeNull();
    }

    private static SqliteConnection CreateOpenConnection()
    {
        var connection = new SqliteConnection("Data Source=:memory:;Mode=Memory;Cache=Shared");
        connection.Open();

        connection.Execute(@"CREATE TABLE IF NOT EXISTS Payments (
            Id TEXT PRIMARY KEY,
            BookingId TEXT NOT NULL,
            Amount REAL NOT NULL,
            PaymentDate TEXT NOT NULL
        )");

        return connection;
    }
}
