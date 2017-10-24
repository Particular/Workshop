using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System;
using System.Threading.Tasks;

namespace ITOps.ViewModelComposition.Gateway
{
    public static class ApplicationBuilderExtensions
    {
        public static void RunCompositionGateway(this IApplicationBuilder app, Action<IRouteBuilder> routes = null)
        {
            var routeBuilder = new RouteBuilder(app);
            routes?.Invoke(routeBuilder);

            app.UseRouter(routeBuilder.Build());
        }

        public static void RunCompositionGatewayWithDefaultRoutes(this IApplicationBuilder app) =>
            app.RunCompositionGateway(routes =>
            {
                routes.MapComposableGet(template: "{controller}/{id:int?}");
                routes.MapRoute("*", context =>
                {
                    context.Response.StatusCode = StatusCodes.Status404NotFound;
                    return Task.CompletedTask;
                });
            });
    }
}
