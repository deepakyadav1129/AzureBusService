using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OrderAPI.Service;

namespace OrderAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly TopicSenderService _topicSenderService;

        public OrdersController(TopicSenderService topicSenderService)
        {
            _topicSenderService = topicSenderService;
        }

        [HttpPost("CreateOrder")]
        public async Task<IActionResult> CreateOrder()
        {
            var order = new
            {
                Id = Guid.NewGuid(),
                ProductName = "Sample Product",
                Quantity = 1,
                Price = 9.99
            };
            await _topicSenderService.PublishOrderAsync(order);
            return Ok(new { Message = "Order created and published to topic / Subscribers." });
        }
    }
}
