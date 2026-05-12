using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Producer.Services;
using System.Text.Json;

namespace Producer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServiceBusController : ControllerBase
    {
        private readonly ServiceBusSenderService _serviceBusSenderService;
        public ServiceBusController(ServiceBusSenderService serviceBusSenderService)
        {
            _serviceBusSenderService = serviceBusSenderService;
        }

        [HttpPost]
        public async Task<IActionResult> SendMessage([FromBody] string data)
        {
            await _serviceBusSenderService.SendMessageAsync(data);
            return Ok("Message sent to Service Bus");
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create()
        {
            var message = new
            {
                OrderId = Guid.NewGuid(),
                Message = "Test Message"
            };
            await _serviceBusSenderService.SendMessageAsync(message);
            return Ok("Message created and sent to Service Bus"); 
        }
    }
}
