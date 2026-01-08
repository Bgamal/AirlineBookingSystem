using System;
using System.Collections.Generic;
using System.Text;

namespace AitlineBookingSystem.BuildingBlocks.Common
{
    public class EventBusConstant
    {
        public const string FlightBookedQueue = "flight-booked-queue";
        public const string PaymentProcessedQueue = "payment-processed-queue";
        public const string NotificationSentQueue = "notification-sent-queue";
    }
}
