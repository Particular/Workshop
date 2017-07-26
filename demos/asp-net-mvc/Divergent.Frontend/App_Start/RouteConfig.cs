using System.Web.Mvc;
using System.Web.Routing;

namespace Divergent.Frontend
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );

            //some IT/Ops code to allow all modules to configure routes: IRegisterRoutes
        }
    }
}
