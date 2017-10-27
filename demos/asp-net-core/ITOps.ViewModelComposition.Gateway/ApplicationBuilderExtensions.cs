using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System;
using System.Threading.Tasks;

namespace ITOps.ViewModelComposition.Gateway
{
    public static class ApplicationBuilderExtensions
    {
        public static void RunCompositionGateway(this IApplicationBuilder app, Action<IRouteBuilder> configureRouteBuilder = null)
        {
            var routeBuilder = new RouteBuilder(app);
            configureRouteBuilder?.Invoke(routeBuilder);

            app.UseRouter(routeBuilder.Build());
        }

        public static void RunCompositionGatewayWithDefaultRoutes(this IApplicationBuilder app) =>
            app.RunCompositionGateway(routeBuilder =>
            {
                routeBuilder.MapComposableGet(template: "{controller}/{id:int?}");
                routeBuilder.MapRoute("*", context =>
                {
                    context.Response.StatusCode = StatusCodes.Status404NotFound;
                    return Task.CompletedTask;
                });
            });
    }
}
