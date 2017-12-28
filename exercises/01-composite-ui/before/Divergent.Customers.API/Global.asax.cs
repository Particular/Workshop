using Radical.Bootstrapper;
using System;
using System.IO;
using System.Web.Http;

namespace Divergent.Customers.API
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            var basePath = AppDomain.CurrentDomain.BaseDirectory;

            var bootstrapper = new WindsorBootstrapper(Path.Combine(basePath, "bin"));
            var container = bootstrapper.Boot();

            GlobalConfiguration.Configure(http => WebApiConfig.Register(http, container));
        }
    }
}
