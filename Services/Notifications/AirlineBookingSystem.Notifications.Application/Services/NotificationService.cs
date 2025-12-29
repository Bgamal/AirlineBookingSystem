using AirlineBookingSystem.Notifications.Application.Interfaces;
using AirlineBookingSystem.Notifications.Core.Entities;
using AitlineBookingSystem.BuildingBlocks.Contracts.EventBus.Messages;
using MassTransit;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace AirlineBookingSystem.Notifications.Application.Services
{
    public class NotificationService : INotificationService
    {
        private readonly IPublishEndpoint _publishEndpoint;
        public NotificationService(IPublishEndpoint publishEndpoint)
        {
            _publishEndpoint = publishEndpoint;
        }
        public async Task SendNotificationAsync(Notification notification)
        {
            // step 1 , will simulate sending a notification via Email or sms
            Console.WriteLine($"Notification sent to {notification.Recipient}:{notification.Message}");
            // step 2 , publish event to message broker
            var notificationEvent = new NotificationEvent
            (
                 notification.Recipient,
                 notification.Message,
                 notification.Type);
            await _publishEndpoint.Publish(notificationEvent);

            // throw new NotImplementedException();
        }
    }
}
