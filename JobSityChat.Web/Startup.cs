using JobSityChat.Common.Settings;
using JobSityChat.Core.Business;
using JobSityChat.Core.Interfaces.Business;
using JobSityChat.Web.Business;
using JobSityChat.Web.Infraestructure.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace JobSityChat.Web
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
            services.AddDbContextConfig(Configuration);
            services.AddTransient<IChatHubService, ChatHubService>();
            services.AddTransient<IMessageHandlerFactory, MessageHandlerFactory>();
            services.Configure<RabbitMQSettings>(Configuration.GetSection("RabbitMQSettings"));
            services.AddSignalR();
            services.AddMassTransitConfig(Configuration);
            services.AddControllersWithViews().AddRazorRuntimeCompilation();
            services.AddRazorPages();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapRazorPages();
                endpoints.MapHub<ChatHubService>("/chatHub");
            });
        }
    }
}
