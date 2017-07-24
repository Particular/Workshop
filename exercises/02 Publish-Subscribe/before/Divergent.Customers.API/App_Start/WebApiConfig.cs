using System.Web.Http;
using System.Linq;
using Newtonsoft.Json.Serialization;
using Castle.Windsor;
using Radical.Bootstrapper.Windsor.AspNet.Infrastructure;
using System.Net.Http.Formatting;

namespace Divergent.Customers.API
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config, IWindsorContainer container)
        {
            config.Formatters.Clear();
            config.Formatters.Add(new JsonMediaTypeFormatter());

            config.DependencyResolver = new WindsorDependencyResolver(container);

            config.Formatters
                .JsonFormatter
                .SerializerSettings
                .ContractResolver = new CamelCasePropertyNamesContractResolver();

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}