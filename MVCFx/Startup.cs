//using Microsoft.AspNetCore.Builder;
//using Microsoft.AspNetCore.DataProtection;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Session;
//using Microsoft.Extensions.Caching.StackExchangeRedis;
//using Microsoft.Extensions.Logging.Abstractions;
//using Microsoft.Extensions.Options;
//using Microsoft.Owin;
//using MVCFx;
//using Owin;
//using System;
//using System.Threading.Tasks;
//using Microsoft.AspNetCore.Http.Features;
//using System.Web;
//using Microsoft.AspNetCore.Http.Internal;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using System.Text;
//using System.Web.UI;
//using Microsoft.AspNetCore.SystemWebAdapters;

//[assembly: OwinStartup(typeof(MVCFx.Startup))]
//namespace MVCFx
//{
//    public class Startup
//    {
//        const string SharedCookie = ".SharedSession";

//        public void Configuration(IAppBuilder app)
//        {
//            var redisCache = new RedisCache(new RedisCacheOptions
//            {
//                Configuration = "localhost:6379",
//                InstanceName = "Session_"
//            });

//            var dataProtectionProvider = DataProtectionProvider.Create("AppFx");
//            var sessionStore = new DistributedSessionStore(redisCache, new NullLoggerFactory());

//            var sessionOptions = new SessionOptions
//            {
//                IdleTimeout = TimeSpan.FromMinutes(30),
//                Cookie = new CookieBuilder
//                {
//                    Name = ".SharedSession",
//                    HttpOnly = true,
//                    IsEssential = true
//                }
//            };

//            // Attach ASP.NET Core Session Middleware to OWIN
//            app.Use(async (context, next) =>
//            {
//                var httpContext = new DefaultHttpContext();
//                var paramContext = new DefaultHttpContext()
//                {
//                    Response =
//                    {
//                        StatusCode = 200,
//                        ContentType = "text/plain",
//                        Body = new MemoryStream(),
//                    }
//                };

//                var cookie = context.Request.Cookies[SharedCookie];
//                if (!string.IsNullOrEmpty(cookie))
//                {
//                    var col = new Dictionary<string, string>()
//                    {
//                        [SharedCookie] = cookie
//                    };
//                    httpContext.Request.Cookies = new Microsoft.AspNetCore.Http.Internal.RequestCookieCollection(col);
//                    paramContext.Request.Cookies = new Microsoft.AspNetCore.Http.Internal.RequestCookieCollection(col);
//                }

//                var sessionMiddleware = new SessionMiddleware(
//                    async ctx =>
//                    {
//                        ISessionFeature feature = new SessionFeature();
//                        feature.Session = ctx.Session;
//                        httpContext.Features.Set(feature);
//                        await ctx.Response.WriteAsync("Hello World!");
//                        var resp = Encoding.UTF8.GetString(((MemoryStream)ctx.Response.Body).ToArray());
//                    },
//                    new NullLoggerFactory(),
//                    dataProtectionProvider,
//                    sessionStore, Options.Create(sessionOptions));

//                await sessionMiddleware.Invoke(paramContext);

//                context.Set("AspNetCoreHttpContext", httpContext);

//                await next.Invoke();
//            });
//        }
//    }
//}