using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Shared;

namespace OrderAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IPublishEndpoint _publishEndpoint;

        public OrdersController(IPublishEndpoint publishEndpoint)
        {
            _publishEndpoint = publishEndpoint;
        }


        [HttpPost]
        public async Task<IActionResult> Post([FromBody] OrderSubmitted orderSubmitted)
        {
            if (orderSubmitted == null || orderSubmitted.OrderId == Guid.Empty)
            {
                return BadRequest("Invalid order data.");
            }

            await _publishEndpoint.Publish(orderSubmitted);
            return Accepted(new { Message = "Order submitted", OrderId = orderSubmitted.OrderId });
        }


    }
}
