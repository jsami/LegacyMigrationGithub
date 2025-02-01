using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Runtime.Remoting.Contexts;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.WebSockets;

namespace MVCFx.Controllers
{
    public class HomeController : Controller
    {
        protected override void OnResultExecuted(ResultExecutedContext filterContext)
        {
            Task.Run(() => SharedSession.CommitAsync()).GetAwaiter().GetResult();
        }

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

        public ISession SharedSession
        {
            get
            {
                var session = HttpContext.Items["SharedSession"] as ISession;
                if (session == null)
                    throw new InvalidOperationException("Shared session not configured");
                return session;
            }
        }

        public async Task<ActionResult> Set(string userName)
        {
            SharedSession.SetString("username", userName);
            return Content("Session Set in .Net FX");
        }

        public ActionResult GetSession()
        {
            var userName = SharedSession.GetString("username");
            return Content($"UserName from .Net FX Session: {userName}");
        }
    }
}