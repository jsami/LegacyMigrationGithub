using BlazorWasm;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

namespace BlazorWasm
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");
            builder.RootComponents.Add<HeadOutlet>("head::after");

            builder.Services.AddScoped(
                sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

            //builder.Services.AddSingleton<INavigationInterception>(
            //    sp => new DisableNavigationInterception());

            await builder.Build().RunAsync();
        }
    }

    public class DisableNavigationInterception : INavigationInterception
    {
        public Task EnableNavigationInterceptionAsync()
        {
            return Task.CompletedTask;
        }
    }
}