using AirlineBookingSystem.Bookings.Application.Commands;
using AirlineBookingSystem.Bookings.Application.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AirlineBookingSystem.Bookings.Api.Controllers
{
    [ApiController]
    [Route("api/bookings")]
    public class BookingController : ControllerBase
    {
        private readonly IMediator _mediator;
        public BookingController(IMediator mediator)
        {
            _mediator = mediator;

        }
        [HttpPost]
        public async Task<IActionResult> AddBooking([FromBody] CreateBookingCommand command)
        {
            var result = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetBookingById), new { id = result }, command);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetBookingById(Guid id)
        {
            var booking = await _mediator.Send(new GetBookingQuery(id));

            if (booking == null)
            {
                return NotFound();
            }
            return Ok(booking);
        }
    }
}
