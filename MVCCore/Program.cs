using System.Reflection.Metadata;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.DataProtection;
using MVCCore.MiddleWare;

namespace MVCCore
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();


            builder.Services.AddReverseProxy()
                .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));
            
            builder.Services.AddDataProtection()
                .PersistKeysToFileSystem(new DirectoryInfo("C:\\SharedDataProtectionKeys"))
                .SetApplicationName("SharedAppName")
                .ProtectKeysWithDpapi();

            builder.Services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = "localhost:6379";
                options.InstanceName = "Session_";
            });

            builder.Services.AddSession(options =>
            {
                options.Cookie.Name = ".SharedSession";
                options.IdleTimeout = TimeSpan.FromMinutes(30);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }
            else
            {
                app.UseWebAssemblyDebugging();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseBlazorFrameworkFiles();

            app.UseSession();

            app.UseRouting();

            app.UseAuthorization();

            app.MapReverseProxy();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.MapFallbackToFile("index.html");

            app.Use(async (ctx, next) =>
            {
                var statusKey = $"__session_init__{ctx.Session.Id}";
                var status = ctx.Session.GetString(statusKey);
                if (string.IsNullOrEmpty(status))
                {
                    await ctx.Session.LoadAsync();
                    ctx.Session.SetString(statusKey, "OK");
                }
                await next(ctx);
            });

            app.Run();
        }
    }
}