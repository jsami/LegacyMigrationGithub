using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MVCCore.Controllers
{
    [Route("api/[controller]")]
    public class CoolServiceController : Controller
    {
        [HttpGet("giveme")]
        public ActionResult GiveMeResponse()
        {
            HttpContext.Session.SetString("service", "cool");
            return Content("This is your response! ");
        }
    }
}
