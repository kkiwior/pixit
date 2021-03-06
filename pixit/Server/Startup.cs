using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Linq;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using pixit.Server.Hubs;
using pixit.Server.Repositiories;
using pixit.Server.Services;
using pixit.Server.Utils;
using StackExchange.Redis.Extensions.Core;
using StackExchange.Redis.Extensions.Core.Configuration;
using StackExchange.Redis.Extensions.Newtonsoft;

namespace pixit.Server
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSignalR(options =>
            {
                options.AddFilter<RoomToHubInvoke>();
            }).AddNewtonsoftJsonProtocol((options) =>
            {
                options.PayloadSerializerSettings.NullValueHandling = NullValueHandling.Ignore;
            });
            services.AddSingleton<ISerializer, NewtonsoftSerializer>();
            JsonConvert.DefaultSettings = () => new JsonSerializerSettings()
            {
                ContractResolver = new RedisContractResolver()
            };
            services.AddStackExchangeRedisExtensions<NewtonsoftSerializer>(Configuration.GetSection("Redis").Get<RedisConfiguration>());
            services.AddControllersWithViews();
            services.AddRazorPages();
            services.AddSingleton<RoomRepository>();
            services.AddSingleton<RoomService>();
            services.AddSingleton<GameService>();
            services.AddResponseCompression(opts =>
            {
                opts.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
                    new[] { "application/octet-stream" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseWebAssemblyDebugging();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UserRedisInformation();
            app.UseHttpsRedirection();
            app.UseBlazorFrameworkFiles();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
                endpoints.MapControllers();
                endpoints.MapHub<RoomHub>("/roomhub");
                endpoints.MapFallbackToFile("index.html");
            });
        }
    }
}
