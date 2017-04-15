using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Routing.Constraints;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace ITOps.ViewModelComposition.Gateway
{
    public static class ApplicationBuilderExtensions
    {
        public static void RunViewModelComposition(this IApplicationBuilder app, Action<IRouteBuilder> routes = null)
        {
            var routeBuilder = new RouteBuilder(app);
            routes?.Invoke(routeBuilder);

            //foreach (var rr in app.ApplicationServices.GetServices<IRegisterRoutes>())
            //{
            //    rr.RegisterRoutes(routeBuilder);
            //}

            app.UseRouter(routeBuilder.Build());
        }

        public static void RunViewModelCompositionWithDefaultRoutes(this IApplicationBuilder app)
        {
            app.RunViewModelComposition(routes =>
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