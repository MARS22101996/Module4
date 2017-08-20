using Cache.WEB;
using Cache.WEB.Jobs.Interfaces;
using Hangfire;
using Microsoft.Owin;
using Owin;
using GlobalConfiguration = System.Web.Http.GlobalConfiguration;

[assembly: OwinStartup(typeof(Startup))]
namespace Cache.WEB
{
    public class Startup
    {
	    private const string DbConnection = "Module4DbConnection";

		public void Configuration(IAppBuilder app)
        {
			Hangfire.GlobalConfiguration.Configuration.UseSqlServerStorage(DbConnection);

			app.UseHangfireDashboard();

			app.UseHangfireServer();

			var initializer = (ISchedulerConfigurer)GlobalConfiguration.Configuration
				.DependencyResolver.GetService(typeof(ISchedulerConfigurer));

			initializer.Configure();
		}
    }
}