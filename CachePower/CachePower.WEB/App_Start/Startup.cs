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
            Hangfire.GlobalConfiguration.Configuration.UseSqlServerStorage("Module4DbConnection");

            app.UseHangfireDashboard();

            app.UseHangfireServer();

            var initializer = (ISchedulerConfigurer)GlobalConfiguration.Configuration
                .DependencyResolver.GetService(typeof(ISchedulerConfigurer));

            initializer.Configure();
        }
    }
}