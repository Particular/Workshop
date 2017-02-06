using Castle.MicroKernel.Registration;
using Divergent.Customers.Data.Migrations;
using Radical.Bootstrapper;
using Raven.Client;
using Raven.Client.Document;
using System;
using System.Configuration;
using System.IO;
using System.Web.Http;

namespace Divergent.Customers.API
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            var basePath = AppDomain.CurrentDomain.BaseDirectory;

            var bootstrapper = new WindsorBootstrapper(Path.Combine(basePath, "bin"), "Divergent*.*");
            var container = bootstrapper.Boot();

            RavenConfig.Config(container);
            GlobalConfiguration.Configure(http => WebApiConfig.Register(http, container));
        }
    }
}
