using AirlineBookingSystem.Notifications.Application.Commands;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AirlineBookingSystem.Notifications.Api.Controllers
{
    [Route("api/Notifcations")]
    [ApiController]
    public class NotifcationsController : ControllerBase
    {
        private readonly IMediator _mediator;
        public NotifcationsController(IMediator mediator)
        {
            _mediator = mediator;

        }
        [HttpPost]
        public async Task<IActionResult> SendNotification([FromBody]SendNotificationCommand command)
        {
            // Placeholder for sending notification logic
            // You would typically send a command/query to the mediator here
          await  _mediator.Send(command);
            return Ok("Notification sent successfully");
        }
    }
}
