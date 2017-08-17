using CachePower.WEB;
using CachePower.WEB.Interfaces;
using Hangfire;
using Microsoft.Owin;
using Owin;
using GlobalConfiguration = System.Web.Http.GlobalConfiguration;

[assembly: OwinStartup(typeof(Startup))]
namespace CachePower.WEB
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            Hangfire.GlobalConfiguration.Configuration.UseSqlServerStorage("ShipmentDbConnection");

            app.UseHangfireDashboard();
            app.UseHangfireServer();

            var initializer = (ISchedulerInitializer)GlobalConfiguration.Configuration
                .DependencyResolver.GetService(typeof(ISchedulerInitializer));
            initializer.Initialize();
        }
    }
}