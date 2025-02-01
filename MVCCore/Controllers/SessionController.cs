using Microsoft.AspNetCore.Mvc;

namespace MVCCore.Controllers
{
    [Route("[controller]")]
    public class SessionController : Controller
    {
        [HttpGet("[action]/{userName}")]
        public IActionResult Set(string userName)
        {
            HttpContext.Session.SetString("username", userName);
            return Content("Session Set in .Net Core");
        }

        [HttpGet("[action]")]
        public IActionResult GetSession()
        {
            var userName = HttpContext.Session.GetString("username");
            return Content($"UserName from .Net Core Session: {userName}");
        }
    }
}
