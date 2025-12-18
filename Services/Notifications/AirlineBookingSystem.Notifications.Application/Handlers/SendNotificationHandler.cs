using AirlineBookingSystem.Notifications.Application.Commands;
using AirlineBookingSystem.Notifications.Application.Interfaces;
using AirlineBookingSystem.Notifications.Core.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace AirlineBookingSystem.Notifications.Application.Handlers
{
    public class SendNotificationHandler : IRequestHandler<SendNotificationCommand>
    {
        private readonly INotificationService _notificationService;
        public SendNotificationHandler(INotificationService notificationService)
        {
                _notificationService = notificationService;
        }
        public async Task Handle(SendNotificationCommand request, CancellationToken cancellationToken)
        {

            var notification = new Notification
            {
                Id= Guid.NewGuid(),
                Recipient = request.Recipient,
                Message = request.Message,
                Type = request.Type
            };
            await _notificationService.SendNotificationAsync(notification);
        }
    }
}
