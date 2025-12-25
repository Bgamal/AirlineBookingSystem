using AitlineBookingSystem.BuildingBlocks.Contracts.EventBus.Messages;
using MassTransit;
using System;
using System.Collections.Generic;
using System.Text;
using static MassTransit.Monitoring.Performance.BuiltInCounters;

namespace AirlineBookingSystem.Bookings.Application.Consumers
{
    public class NotificationEventConsumer : IConsumer<NotificationEvent>
    {
        public async Task Consume(ConsumeContext<NotificationEvent> context)
        {
            var message = context.Message;
            // Process the notification event (e.g., log it, update database, etc.)
            Console.WriteLine($"Notification received for Recipient : {message.Recipient}, Status: {message.Message}, Type: {message.Type}");
            // Acknowledge the message processing is complete
            await Task.CompletedTask;
        }
    }
}
