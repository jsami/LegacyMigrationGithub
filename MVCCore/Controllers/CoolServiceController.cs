using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MVCCore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CoolServiceController : ControllerBase
    {
        [HttpGet("giveme")]
        public IActionResult GiveMeResponse()
        {
            return Ok(new { message = "This is your response! " });
        }
    }
}
