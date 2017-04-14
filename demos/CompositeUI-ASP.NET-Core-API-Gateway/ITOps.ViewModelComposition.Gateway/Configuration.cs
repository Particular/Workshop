using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Routing.Constraints;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using System.Threading.Tasks;

namespace ITOps.ViewModelComposition.Gateway
{
    public static class ViewModelCompositionConfigurationExtensions
    {
        public static void AddViewModelComposition(this IServiceCollection services, string assemblySearchPattern = "*ViewModelComposition*.dll")
        {
            var fileNames = Directory.GetFiles(AppContext.BaseDirectory, assemblySearchPattern);

            var types = new List<Type>();
            foreach (var fileName in fileNames)
            {
                var temp = Assembly.Load(new AssemblyName(Path.GetFileNameWithoutExtension(fileName))).GetTypes().Where(t =>
                {
                    return !t.GetTypeInfo().IsInterface &&
                    (
                        typeof(IRouteInterceptor).IsAssignableFrom(t)
                        || typeof(IRegisterRoutes).IsAssignableFrom(t)
                    );
                });

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

        public static void UseViewModelCompositionGateway(this IApplicationBuilder app, IRouteBuilder routeBuilder = null)
        {
            var ownBuilder = false;
            if (routeBuilder == null)
            {
                routeBuilder = new RouteBuilder(app);
                ownBuilder = true;
            }

            var interceptors = app.ApplicationServices.GetServices<IRouteInterceptor>();
            var routeHandler = new RouteHandler(ctx => HandleRouteRequest(ctx, interceptors));
            var route = new Route(
                routeHandler,
                "compose/{controller=Home}/{action=Index}/{id?}",
                defaults: null,
                constraints: new RouteValueDictionary(new { httpMethod = new HttpMethodRouteConstraint("GET") }),
                dataTokens: null,
                inlineConstraintResolver: app.ApplicationServices.GetRequiredService<IInlineConstraintResolver>()
            );

            routeBuilder.Routes.Add(route);

            foreach (var rr in app.ApplicationServices.GetServices<IRegisterRoutes>())
            {
                rr.RegisterRoutes(routeBuilder, routeHandler);
            }

            if (ownBuilder)
            {
                var routes = routeBuilder.Build();
                app.UseRouter(routes);
            }
        }

        private static async Task HandleRouteRequest(HttpContext context, IEnumerable<IRouteInterceptor> routeInterceptors)
        {
            var vm = new DynamicViewModel(context);

            try
            {
                var pending = new List<Task>();

                //matching interceptors could be cached by URL
                foreach (var interceptor in routeInterceptors.Where(a => a.Matches(context.GetRouteData())))
                {
                    if (interceptor is ISubscribeToCompositionEvents subscriber)
                    {
                        subscriber.Subscribe(vm, context.GetRouteData(), context.Request.Query);
                    }

                    if (interceptor is IViewModelAppender appender)
                    {
                        var task = appender.Append(vm, context.GetRouteData(), context.Request.Query);
                        pending.Add(task);
                    }

                    //if there are subscribers but no appenders should we throw? IMO, yes.
                }

                if (pending.Count > 0)
                {
                    await Task.WhenAll(pending.ToArray());
                    var json = JsonConvert.SerializeObject(vm);
                    await context.Response.WriteAsync(json);
                }
            }
            finally
            {
                vm.CleanupSubscribers();
            }
        }
    }
}
