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

            //foreach (var rr in app.ApplicationServices.GetServices<IRegisterRoutes>())
            //{
            //    rr.RegisterRoutes(routeBuilder);
            //}

            app.UseRouter(routeBuilder.Build());
        }

        public static void RunCompositionGatewayWithDefaultRoutes(this IApplicationBuilder app)
        {
            app.RunCompositionGateway(routes =>
            {
                routes.MapComposableGet( template: "{controller}/{id:int?}");
                routes.MapRoute("{*NotFound}", context =>
                 {
                     context.Response.StatusCode = StatusCodes.Status404NotFound;
                     return Task.CompletedTask;
                 });
            });
        }
    }
}