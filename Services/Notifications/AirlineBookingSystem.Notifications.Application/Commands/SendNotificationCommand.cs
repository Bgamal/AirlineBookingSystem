using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace AirlineBookingSystem.Notifications.Application.Commands
{
    public record SendNotificationCommand(
       string Recipient,
        string Message,
       string Type
    ):IRequest;
   
}
