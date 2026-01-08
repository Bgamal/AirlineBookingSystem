using AirlineBookingSystem.Bookings.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace AirlineBookingSystem.Bookings.Core.Repositories
{
    public interface IBookingRepository
    {
        Task<Booking?> GetBookingByIdAsync(Guid bookingId);
        Task AddBookingAsync(Booking booking);
    }
}
