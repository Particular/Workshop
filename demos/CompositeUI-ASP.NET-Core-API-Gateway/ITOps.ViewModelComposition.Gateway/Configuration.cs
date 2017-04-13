using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;

namespace ITOps.ViewModelComposition.Gateway
{
    public static class ViewModelCompositionConfigurationExtensions
    {
        public static void AddViewModelComposition(this IServiceCollection services)
        {
            var fileNames = Directory.GetFiles(AppContext.BaseDirectory, "*ViewModelComposition*.dll");

            var types = new List<Type>();
            foreach (var fileName in fileNames)
            {
                var temp = Assembly.Load(new AssemblyName(Path.GetFileNameWithoutExtension(fileName))).GetTypes().Where(t =>
                    (typeof(IRouteInterceptor).IsAssignableFrom(t) && t != typeof(IRouteInterceptor))
                    || (typeof(IRegisterRoutes).IsAssignableFrom(t) && t != typeof(IRegisterRoutes))
                    );

                types.AddRange(temp);
            }

            foreach (var type in types)
            {
                if (typeof(IRegisterRoutes).IsAssignableFrom(type))
                {
                    services.AddSingleton(typeof(IRegisterRoutes), type);
                }

                if (typeof(IRouteInterceptor).IsAssignableFrom(type))
                {
                    services.AddSingleton(typeof(IRouteInterceptor), type);
                }
            }
        }

        public static void UseViewModelCompositionGateway(this IApplicationBuilder app)
        {
            var defaultGatewayRouteHandler = new RouteHandler(context =>
            {
                //look for the appender

                var routeValues = context.GetRouteData().Values;
                return context.Response.WriteAsync(
                    $"Hello! Route values: {string.Join(", ", routeValues)}");
            });

            var routeBuilder = new RouteBuilder(app, defaultGatewayRouteHandler);

            routeBuilder.MapRoute(
                name: "API Gateway",
                template: "{controller=Home}/{action=Index}/{id?}"
            );

            foreach (var rr in app.ApplicationServices.GetServices<IRegisterRoutes>())
            {
                rr.RegisterRoutes(routeBuilder, defaultGatewayRouteHandler);
            }

            //routeBuilder.MapGet("hello/{name}", context =>
            //{
            //    var name = context.GetRouteValue("name");
            //    // This is the route handler when HTTP GET "hello/<anything>"  matches
            //    // To match HTTP GET "hello/<anything>/<anything>,
            //    // use routeBuilder.MapGet("hello/{*name}"
            //    return context.Response.WriteAsync($"Hi, {name}!");
            //});

            var routes = routeBuilder.Build();
            app.UseRouter(routes);
        }
    }
}
