using System.Net.Http.Formatting;
using System.Web.Http;
using System.Web.Http.Cors;
using Castle.Windsor;
using Newtonsoft.Json.Serialization;
using Owin;
using Radical.Bootstrapper.Windsor.AspNet.Infrastructure;

namespace Divergent.Sales.API
{
    public class WebApiConfig
    {
        public static void Configure(IAppBuilder app, IWindsorContainer container)
        {
            var config = new HttpConfiguration();

            config.MapHttpAttributeRoutes();
            config.EnableCors(new EnableCorsAttribute("*", "*", "*"));

            config.DependencyResolver = new WindsorDependencyResolver(container);

            config.Formatters.Clear();
            config.Formatters.Add(new JsonMediaTypeFormatter());

            config.Formatters
                .JsonFormatter
                .SerializerSettings
                .ContractResolver = new CamelCasePropertyNamesContractResolver();

            app.UseWebApi(config);
        }
    }
}
