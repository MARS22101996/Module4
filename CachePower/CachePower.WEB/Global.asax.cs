﻿using System.Web.Http;

namespace CachePower.WEB
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