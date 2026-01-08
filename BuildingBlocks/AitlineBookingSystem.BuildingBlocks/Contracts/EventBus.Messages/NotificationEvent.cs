using System;
using System.Collections.Generic;
using System.Text;

namespace AitlineBookingSystem.BuildingBlocks.Contracts.EventBus.Messages
{
    public record NotificationEvent
        (
        string Recipient,
        string Message,
        string Type
        );
   
}
