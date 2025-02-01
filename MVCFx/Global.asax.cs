using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Builder.Internal;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Microsoft.AspNetCore.DataProtection;

namespace MVCFx
{
    public class MvcApplication : System.Web.HttpApplication
    {
        private static RequestDelegate _aspNetCorePipeline;

        public ISession SharedSession
        {
            get
            {
                var session = Context.Items["SharedSession"] as ISession;
                if (session == null)
                    throw new InvalidOperationException("Shared session not configured");
                return session;
            }
        }

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

            services.AddDataProtection()
                .PersistKeysToFileSystem(new DirectoryInfo("C:\\SharedDataProtectionKeys"))
                .SetApplicationName("SharedAppName")
                .ProtectKeysWithDpapi();

            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = "localhost:6379";
                options.InstanceName = "Session_";
            });

            services.AddSession(options =>
            {
                options.Cookie.Name = ".SharedSession";
                options.IdleTimeout = TimeSpan.FromMinutes(30);
            });

            var serviceProvider = services.BuildServiceProvider();

            var builder = new ApplicationBuilder(serviceProvider);

            builder.UseSession();
            
            builder.Run(context =>
            {
                var mvcContext = context.Items["AspNetMvcContext"] as HttpContextBase;
                if (mvcContext != null)
                {
                    mvcContext.Items["SharedSession"] = context.Session;
                }

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
                context.Items["AspNetMvcContext"] = mvcContext;

                var cookieHeader = mvcContext.Request.Headers["Cookie"];
                if (!string.IsNullOrEmpty(cookieHeader))
                {
                    context.Request.Headers["Cookie"] = cookieHeader;
                }
                
                Task.Run(() =>  _aspNetCorePipeline(context)).GetAwaiter().GetResult();
            }
        }

        protected void Application_EndRequest(object sender, EventArgs e)
        {
            Task.Run(() => SharedSession.CommitAsync()).GetAwaiter().GetResult();
        }
    }
}
