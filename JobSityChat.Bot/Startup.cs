using JobSityChat.Bot.Infraestructure;
using JobSityChat.Bot.Infraestructure.Extensions;
using JobSityChat.Bot.Infraestructure.Middleware;
using JobSityChat.Common.Settings;
using JobSityChat.Core.Proxies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace JobSityChat.Bot
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddScoped<IStockMarketProxy, StockMarketProxy>();
            services.Configure<RabbitMQSettings>(Configuration.GetSection("RabbitMQSettings"));
            services.Configure<AppSettings>(Configuration.GetSection("AppSettings"));
            services.AddHttpClients(Configuration);
            services.AddMassTransitConfig(Configuration);
            services.AddControllers();            
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseMiddleware<BotExceptionMiddleware>();
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();               
            });

        }
    }
}
