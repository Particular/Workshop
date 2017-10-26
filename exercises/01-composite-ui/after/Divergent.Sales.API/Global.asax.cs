using System;
using System.IO;
using System.Web.Http;
using Castle.MicroKernel.Registration;
using Divergent.Sales.Data.Repositories;
using Radical.Bootstrapper;

namespace Divergent.Sales.API
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            var basePath = AppDomain.CurrentDomain.BaseDirectory;

            var bootstrapper = new WindsorBootstrapper(Path.Combine(basePath, "bin"));
            var container = bootstrapper.Boot();

            var dataManagerComponent = Component.For<IOrderRepository>()
                .Instance(new OrderRepository())
                .LifestyleSingleton();

            container.Register(dataManagerComponent);

            GlobalConfiguration.Configure(http => WebApiConfig.Register(http, container));
        }
    }
}
