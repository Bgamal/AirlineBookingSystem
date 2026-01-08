using AirlineBookingSystem.Bookings.Application.Commands;
using AirlineBookingSystem.Bookings.Core.Entities;
using AirlineBookingSystem.Bookings.Core.Repositories;
using AitlineBookingSystem.BuildingBlocks.Contracts.EventBus.Messages;
using MassTransit;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace AirlineBookingSystem.Bookings.Application.Handlers
{
    public class CreateBookingHandler : IRequestHandler<CreateBookingCommand, Guid>
    {
        private readonly IBookingRepository _bookingRepository;
        private readonly IPublishEndpoint _publishEndpoint;
        public CreateBookingHandler(IBookingRepository bookingRepository, IPublishEndpoint publishEndpoint)
        {
            _bookingRepository = bookingRepository;
            _publishEndpoint = publishEndpoint;
        }

        public async Task<Guid> Handle(CreateBookingCommand request, CancellationToken cancellationToken)
        {
            var booking = new Booking
            {
                Id = Guid.NewGuid(),
                FlightId = request.FlightId,
                PassengerName = request.PassengerName,
                SeatNumber = request.SeatNumber,
                BookingDate = DateTime.UtcNow
            };
            await _bookingRepository.AddBookingAsync(booking);

            //publish FlightBooked event
            await _publishEndpoint.Publish(new FlightBookedEvent(
                booking.Id,
                booking.FlightId,
                booking.PassengerName,
                booking.SeatNumber,
                booking.BookingDate
            ));


            return booking.Id;
        }
    }
}
