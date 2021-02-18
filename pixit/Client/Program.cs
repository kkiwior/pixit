using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Blazored.LocalStorage;
using Mapster;
using pixit.Client.Services;
using pixit.Client.Utils;

namespace pixit.Client
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.Services.AddScoped<SignalRService>();
            builder.Services.AddScoped<SendEventService>();
            builder.Services.AddScoped<Mediator>();
            builder.Services.AddSingleton<StateContainer>();
            builder.Services.AddBlazoredLocalStorage();
            builder.RootComponents.Add<App>("#app");

            builder.Services.AddScoped(_ => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

            TypeAdapterConfig.GlobalSettings.Default.PreserveReference(true);


            await builder.Build().RunAsync();
        }
    }
}
