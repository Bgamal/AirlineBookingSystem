using AirlineBookingSystem.Notifications.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace AirlineBookingSystem.Notifications.Core.Repositories
{
    public interface INotificationRepository
    {
        Task LogNotificationAsync(Notification notification);
        Task<Notification> GetNotificationByIdAsync(Guid notificationId);
        Task AddNotificationAsync(Notification notification);
        Task<IEnumerable<Notification>> GetAllNotificationsAsync();
        Task DeleteNotificationAsync(Guid notificationId);
    }
}
