using AirlineBookingSystem.Payments.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace AirlineBookingSystem.Payments.Core.Repositories
{
    public interface IPaymentRepositry
    {
        Task<Payment> GetPaymentByIdAsync(Guid paymentId);
        Task<Payment> RefundPaymentAsync(Guid paymentId);
        Task ProcessPaymentAsync(Payment payment);
        Task<IEnumerable<Payment>> GetAllPaymentsAsync();
        Task DeletePaymentAsync(Guid paymentId);
    }
}
