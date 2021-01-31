using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using pixit.Client.Services;
using pixit.Client.Shared;
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
            builder.Services.AddBlazoredLocalStorage();
            builder.RootComponents.Add<App>("#app");

            builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

            //CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("en-US");
            

            await builder.Build().RunAsync();
        }
    }
}
