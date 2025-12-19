using AirlineBookingSystem.Payments.Application.Commands;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AirlineBookingSystem.Payments.Api.Controllers
{
    [Route("api/Payments")]
    [ApiController]
    public class PaymentsController : ControllerBase
    {
        private readonly IMediator _mediator;
        public PaymentsController(IMediator mediator)
        {
            _mediator = mediator;

        }
        [HttpPost] 
        public async Task<IActionResult> ProcessPayment([FromBody] ProcessPaymentCommand command)
        {
            // Placeholder for payment processing logic
            // You would typically send a command/query to the mediator here
           var id =  await _mediator.Send(command);
            return CreatedAtAction(nameof(ProcessPayment), new { id }, command);
        }
        [HttpPost("refund/{id}")]
        public async Task<IActionResult> RefundPayment(Guid id)
        {
            // Placeholder for refund processing logic
            // You would typically send a command/query to the mediator here
            await _mediator.Send(new RefundPaymentCommand(id));
            return NoContent();
        }
    }
}
