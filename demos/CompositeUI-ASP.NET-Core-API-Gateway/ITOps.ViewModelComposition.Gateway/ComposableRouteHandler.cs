using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ITOps.ViewModelComposition.Gateway
{
    public class ComposableRouteHandler
    {
        public static async Task HandleGetRequest(HttpContext context)
        {
            var vm = new DynamicViewModel(context);
            var pending = new List<Task>();
            var routeData = context.GetRouteData();
            var interceptors = context.RequestServices.GetServices<IRouteInterceptor>();

            try
            {
                //matching interceptors could be cached by URL
                var matching = interceptors
                    .Where(a => a.Matches(context.GetRouteData(), HttpMethods.Get))
                    .ToArray();

                foreach (var subscriber in matching.OfType<ISubscribeToCompositionEvents>())
                {
                    subscriber.Subscribe(vm, routeData, context.Request.Query);
                }

                foreach (var appender in matching.OfType<IViewModelAppender>())
                {
                    pending.Add
                    (
                        appender.Append(vm, routeData, context.Request.Query)
                    );
                }

                if (pending.Count == 0)
                {
                    context.Response.StatusCode = StatusCodes.Status404NotFound;
                }
                else
                {
                    await Task.WhenAll(pending.ToArray());
                    var json = JsonConvert.SerializeObject(vm, GetSettings(context));
                    await context.Response.WriteAsync(json);
                }
            }
            finally
            {
                vm.CleanupSubscribers();
            }
        }

        static JsonSerializerSettings GetSettings(HttpContext context)
        {
            if (!context.Request.Headers.TryGetValue("Accept-Casing", out StringValues casing))
            {
                casing = "casing/camel";
            }

            switch (casing)
            {
                case "casing/pascal":
                    return new JsonSerializerSettings();

                default: // "casing/camel":
                    return new JsonSerializerSettings()
                    {
                        ContractResolver = new CamelCasePropertyNamesContractResolver()
                    };
            }
        }
    }
}
