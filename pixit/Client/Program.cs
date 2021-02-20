using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Threading.Tasks;
using Blazored.LocalStorage;
using Mapster;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Localization;
using Microsoft.JSInterop;
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

            builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");
            builder.RootComponents.Add<App>("#app");

            TypeAdapterConfig.GlobalSettings.Default.PreserveReference(true);
            builder.Services.AddScoped(_ => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

            var host = builder.Build();
            var jsInterop = host.Services.GetRequiredService<IJSRuntime>();
            var result = await jsInterop.InvokeAsync<string>("blazorCulture.get");
            var culture =  new CultureInfo(result ?? "en");
            culture.NumberFormat.NumberDecimalSeparator = ".";
            CultureInfo.DefaultThreadCurrentCulture = culture;
            CultureInfo.DefaultThreadCurrentUICulture = culture;

            await host.RunAsync();
        }
    }
}
