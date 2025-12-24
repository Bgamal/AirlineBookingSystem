using AirlineBookingSystem.Notifications.Core.Entities;
using AirlineBookingSystem.Notifications.Infrastructure.Repositories;
using Dapper;
using FluentAssertions;
using Microsoft.Data.Sqlite;
using Xunit;

namespace AirlineBookingSystem.Notifications.Infrastructure.Tests.Repositories;

public class NotificationRepositoryTests
{
    static NotificationRepositoryTests()
    {
        SqlMapper.AddTypeHandler(new SqliteGuidTypeHandler());
    }

    [Fact]
    public async Task AddNotificationAsync_ShouldPersistNotification()
    {
        // Arrange
        await using var connection = CreateOpenConnection();
        var repository = new NotificationRepository(connection);
        var notification = new Notification
        {
            Id = Guid.NewGuid(),
            Recipient = "user@example.com",
            Message = "Hello",
            Type = "Email"
        };

        // Act
        await repository.AddNotificationAsync(notification);

        // Assert
        var stored = await connection.QuerySingleOrDefaultAsync<Notification>(
            "SELECT * FROM Notifications WHERE Id = @Id",
            new { notification.Id });

        stored.Should().NotBeNull();
        stored!.Should().BeEquivalentTo(notification);
    }

    [Fact]
    public async Task GetNotificationByIdAsync_WhenExists_ShouldReturnNotification()
    {
        // Arrange
        await using var connection = CreateOpenConnection();
        var repository = new NotificationRepository(connection);

        var notification = new Notification
        {
            Id = Guid.NewGuid(),
            Recipient = "user@example.com",
            Message = "Hello",
            Type = "Email"
        };

        await connection.ExecuteAsync(
            "INSERT INTO Notifications (Id, recipient, Message, Type) VALUES (@Id, @Recipient, @Message, @Type)",
            notification);

        // Act
        var result = await repository.GetNotificationByIdAsync(notification.Id);

        // Assert
        result.Should().NotBeNull();
        result!.Should().BeEquivalentTo(notification);
    }

    [Fact]
    public async Task DeleteNotificationAsync_ShouldRemoveNotification()
    {
        // Arrange
        await using var connection = CreateOpenConnection();
        var repository = new NotificationRepository(connection);

        var notification = new Notification
        {
            Id = Guid.NewGuid(),
            Recipient = "user@example.com",
            Message = "Hello",
            Type = "Email"
        };

        await connection.ExecuteAsync(
            "INSERT INTO Notifications (Id, recipient, Message, Type) VALUES (@Id, @Recipient, @Message, @Type)",
            notification);

        // Act
        await repository.DeleteNotificationAsync(notification.Id);

        // Assert
        var result = await connection.QuerySingleOrDefaultAsync<Notification>(
            "SELECT * FROM Notifications WHERE Id = @Id",
            new { notification.Id });

        result.Should().BeNull();
    }

    private static SqliteConnection CreateOpenConnection()
    {
        var connection = new SqliteConnection("Data Source=:memory:;Mode=Memory;Cache=Shared");
        connection.Open();

        connection.Execute(@"CREATE TABLE IF NOT EXISTS Notifications (
            Id TEXT PRIMARY KEY,
            recipient TEXT NOT NULL,
            Message TEXT NOT NULL,
            Type TEXT NOT NULL
        )");

        return connection;
    }
}
