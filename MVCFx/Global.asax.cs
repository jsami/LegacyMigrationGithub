using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Builder.Internal;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace MVCFx
{
    public class MvcApplication : System.Web.HttpApplication
    {
        private static RequestDelegate _aspNetCorePipeline;

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            BuildRequestPipeline();
        }

        private static void  BuildRequestPipeline()
        {
            var services = new ServiceCollection();

            services.AddLogging();

            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = "localhost:6379";
                options.InstanceName = "Session_";
            });

            services.AddSession(options =>
            {
                options.Cookie.Name = ".SharedSession";
                options.IdleTimeout = TimeSpan.FromMinutes(30);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            var serviceProvider = services.BuildServiceProvider();

            var builder = new ApplicationBuilder(serviceProvider);

            builder.UseSession();
            
            builder.Run((context) =>
            {
                Console.WriteLine("It Works!");
                return Task.CompletedTask;
            });

            _aspNetCorePipeline = builder.Build();
        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            if (_aspNetCorePipeline != null)
            {
                var context = new DefaultHttpContext();
                HttpContextBase mvcContext = new HttpContextWrapper(Context);

                var cookieHeader = mvcContext.Request.Headers["Cookie"];
                if (!string.IsNullOrEmpty(cookieHeader))
                {
                    context.Request.Headers["Cookie"] = cookieHeader;
                    context.Items["AspNetMvcContext"] = mvcContext;
                }
                
                Task.Run(() =>  _aspNetCorePipeline(context)).GetAwaiter().GetResult();
            }
        }
    }
}
