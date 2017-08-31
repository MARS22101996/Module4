using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(Auth.Host.Startup))]

namespace Auth.Host
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {

			ConfigureAuth(app);
        }
    }
}
