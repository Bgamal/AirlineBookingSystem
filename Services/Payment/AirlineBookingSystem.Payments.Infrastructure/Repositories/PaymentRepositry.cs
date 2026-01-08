using AirlineBookingSystem.Payments.Core.Entities;
using AirlineBookingSystem.Payments.Core.Repositories;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace AirlineBookingSystem.Payments.Infrastructure.Repositories
{
    public class PaymentRepositry : IPaymentRepositry
    {
        private readonly IDbConnection _dbConnection;
        public PaymentRepositry(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task DeletePaymentAsync(Guid paymentId)
        {
            const string sql = "DELETE FROM Payments WHERE Id = @Id";
            await _dbConnection.ExecuteAsync(sql, new { Id = paymentId });
        }

        public async Task<IEnumerable<Payment>> GetAllPaymentsAsync()
        {
            const string sql = "SELECT * FROM Payments";
            return await _dbConnection.QueryAsync<Payment>(sql);
        }

        public async Task<Payment> GetPaymentByIdAsync(Guid paymentId)
        {
            const string sql = "SELECT * FROM Payments WHERE Id = @Id";
            return await _dbConnection.QuerySingleOrDefaultAsync<Payment>(sql, new { Id = paymentId });
        }

        public async Task ProcessPaymentAsync(Payment payment)
        {
            const string sql = @"INSERT INTO Payments (Id, BookingId, Amount, PaymentDate)
                                 VALUES (@Id, @BookingId, @Amount, @PaymentDate)";
            await _dbConnection.ExecuteAsync(sql, payment);
        }

        public async Task<Payment> RefundPaymentAsync(Guid paymentId)
        {
            var payment = await GetPaymentByIdAsync(paymentId);
            if (payment == null)
            {
                return null;
            }

            return payment;
        }
    }
}
