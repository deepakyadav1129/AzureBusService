using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Producer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DLQController : ControllerBase
    {
        private readonly Services.DlqService _dlqService;

        public DLQController(Services.DlqService dlqService)
        {
            _dlqService = dlqService;
        }

        [HttpGet]
        public async Task<IActionResult> GetDlqMessages()
        {
            var dlqMessages = await _dlqService.GetDlqMessageAsync();
            return Ok(dlqMessages);
        }
    }
}
