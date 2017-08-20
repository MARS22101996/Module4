using System.Web.Http;
using Cache.WEB.Infrastructure.DI;

namespace Cache.WEB
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);
            ConfigDI.Setup();
        }
    }
}