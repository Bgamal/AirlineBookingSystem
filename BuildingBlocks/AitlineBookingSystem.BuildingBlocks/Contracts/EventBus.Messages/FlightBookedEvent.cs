using System;
using System.Collections.Generic;
using System.Text;

namespace AitlineBookingSystem.BuildingBlocks.Contracts.EventBus.Messages
{
    public record FlightBookedEvent
        (
        Guid BookingId,
        Guid FlightId,
        string PassengerName,
        string SeatNumber,
        DateTime BookingDate
        );


}
