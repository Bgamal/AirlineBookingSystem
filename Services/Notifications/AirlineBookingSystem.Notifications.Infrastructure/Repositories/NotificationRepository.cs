using AirlineBookingSystem.Notifications.Core.Entities;
using AirlineBookingSystem.Notifications.Core.Repositories;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace AirlineBookingSystem.Notifications.Infrastructure.Repositories
{
    public class NotificationRepository : INotificationRepository
    {
        private readonly IDbConnection _dbConnection;
        public NotificationRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task AddNotificationAsync(Notification notification)
        {
            const string sql = @"INSERT INTO Notifications (Id, recipient, Message, Type)
                                 VALUES (@Id, @recipient, @Message, @Type)";
            await _dbConnection.ExecuteAsync(sql, notification);
        }

        public async Task DeleteNotificationAsync(Guid notificationId)
        {
            const string sql = "DELETE FROM Notifications WHERE Id = @Id";
            await _dbConnection.ExecuteAsync(sql, new { Id = notificationId });
        }

        public async Task<IEnumerable<Notification>> GetAllNotificationsAsync()
        {
            const string sql = "SELECT * FROM Notifications";
            return await _dbConnection.QueryAsync<Notification>(sql);
        }

        public async Task<Notification> GetNotificationByIdAsync(Guid notificationId)
        {
            const string sql = "SELECT * FROM Notifications WHERE Id = @Id";
            return await _dbConnection.QuerySingleOrDefaultAsync<Notification>(sql, new { Id = notificationId });
        }

        public async Task LogNotificationAsync(Notification notification)
        {
            // Here LogNotificationAsync could be an alias for AddNotificationAsync or may include extra logging behavior.
            await AddNotificationAsync(notification);
        }
    }
}
