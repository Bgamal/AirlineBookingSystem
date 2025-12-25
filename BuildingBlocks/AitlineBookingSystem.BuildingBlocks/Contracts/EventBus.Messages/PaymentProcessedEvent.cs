using System;
using System.Collections.Generic;
using System.Text;

namespace AitlineBookingSystem.BuildingBlocks.Contracts.EventBus.Messages
{
    public record PaymentProcessedEvent
        (
        Guid PaymentId,
        Guid BookingId,
        decimal Amount,
        DateTime PaymentDate
        );
   
}
