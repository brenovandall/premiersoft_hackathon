using Microsoft.AspNetCore.Mvc;

namespace Hackathon.Premiersoft.API.Controllers
{
    [ApiController]
    [Route("v1/[controller]")]
    public class DataController : ControllerBase
    {
        [HttpGet("health")]
        public IActionResult Health()
        {
            return Ok(new { status = "API GetData está funcionando", timestamp = DateTime.UtcNow });
        }

        [HttpGet("test")]
        public IActionResult Test()
        {
            return Ok(new { message = "Endpoint de teste funcionando" });
        }
    }
}