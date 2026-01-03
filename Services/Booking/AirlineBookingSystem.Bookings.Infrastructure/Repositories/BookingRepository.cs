using AirlineBookingSystem.Bookings.Core.Entities;
using AirlineBookingSystem.Bookings.Core.Repositories;
using Dapper;
using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Data;
using System.Net.Http.Json;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace AirlineBookingSystem.Bookings.Infrastructure.Repositories
{
    public class BookingRepository : IBookingRepository
    {
        private readonly IDbConnection _dbConnection;

        private readonly IDatabase _redisDatabase;
        private const string RedisKeyPrefix = "booking_";
        public BookingRepository(IConnectionMultiplexer redisConnection)
        {
                _redisDatabase = redisConnection.GetDatabase();
        }
        //public BookingRepository(IDbConnection dbConnection)
        //{
        //    _dbConnection = dbConnection;

        //}
        public async Task AddBookingAsync(Booking booking)
        {
            //const string sql = @"
            //    INSERT INTO Bookings (Id, FlightId, PassengerName, BookingDate, SeatNumber)
            //    VALUES (@Id, @FlightId, @PassengerName, @BookingDate, @SeatNumber)";
            //await _dbConnection.ExecuteAsync(sql, booking);

            var data = JsonConvert.SerializeObject(booking);
            await _redisDatabase.StringSetAsync($"{RedisKeyPrefix}{booking.Id}" , data);
        }

        public async Task<Booking?> GetBookingByIdAsync(Guid bookingId)
        {
            //const string sql = "SELECT * FROM Bookings WHERE Id = @Id";
            //return await _dbConnection.QuerySingleOrDefaultAsync<Booking>(sql, new { Id = bookingId });

            var data = await _redisDatabase.StringGetAsync($"{RedisKeyPrefix}{bookingId}");
            if (data.IsNullOrEmpty)
            {
                return null;
            }
            return JsonConvert.DeserializeObject<Booking>(data);
        }
    }
}
