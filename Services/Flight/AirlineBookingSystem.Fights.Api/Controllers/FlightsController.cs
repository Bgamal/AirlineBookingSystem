using AirlineBookingSystem.Fights.Application.Commands;
using AirlineBookingSystem.Fights.Application.Handlers;
using AirlineBookingSystem.Fights.Application.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AirlineBookingSystem.Fights.Api.Controllers
{
    [ApiController]
    [Route("api/Flights")]
    public class FlightsController : ControllerBase
    {
        private readonly IMediator _mediator;
        public FlightsController(IMediator mediator)
        {
            _mediator = mediator;

        }
        [HttpGet]
        public async Task<IActionResult> GetFlights()
        {
            var flights = await _mediator.Send(new GatAllFlightsQuery());
            return Ok(flights);
        }

        [HttpPost]
        public async Task<IActionResult> AddFlight([FromBody] CreateFlightCommand command)
        {
            var result = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetFlights), new { id = result }, command);
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFlight(Guid id)
        {
            await _mediator.Send(new DeleteFlightCommand(id));
            return NoContent();
        }
    }
}
