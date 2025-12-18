using AirlineBookingSystem.Notifications.Application.Interfaces;
using AirlineBookingSystem.Notifications.Core.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace AirlineBookingSystem.Notifications.Application.Services
{
    public class NotificationService : INotificationService
    {
        public async Task SendNotificationAsync(Notification notification)
        {
            // step 1 , will simulate sending a notification via Email or sms
            Console.WriteLine($"Notification sent to {notification.Recipient}:{notification.Message}");

            // throw new NotImplementedException();
        }
    }
}
