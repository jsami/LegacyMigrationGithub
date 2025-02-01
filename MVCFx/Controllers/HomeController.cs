using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.WebSockets;

namespace MVCFx.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            return View();
        }

        public ActionResult Contact()
        {
            return View();
        }

        protected HttpContext AspNetCoreContext
        {
            get
            {
                var owinEnvironment = HttpContext.Items["owin.Environment"] as IDictionary<string, object>;
                if (owinEnvironment != null)
                {
                    var context = owinEnvironment["AspNetCoreHttpContext"] as DefaultHttpContext;
                    return context;
                }

                return null;
            }
        }

        public ActionResult Set(string userName)
        {
            AspNetCoreContext?.Session.SetString("username", userName);
            return Content("Session Set in .Net FX");
        }

        public ActionResult GetSession()
        {
            var userName = AspNetCoreContext?.Session.GetString("username");
            return Content($"UserName from .Net FX Session: {userName}");
        }
    }
}