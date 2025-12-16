using System;
using System.Collections.Generic;
using System.Text;

namespace AirlineBookingSystem.Payments.Core.Entities
{
    public class Payment
    {
        public Guid Id { get; set; }
        public Guid BookingId { get; set; }
        public decimal Amount { get; set; }
        public decimal Balance { get; set; }
        public DateTime PaymentDate { get; set; }

    }
}
