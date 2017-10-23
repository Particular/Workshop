using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ITOps.ViewModelComposition
{
    public class CompositionHandler
    {
        public static async Task<(dynamic ViewModel, int StatusCode)> HandleGetRequest(HttpContext context)
        {
            var vm = new DynamicViewModel(context);
            var pending = new List<Task>();
            var routeData = context.GetRouteData();
            var interceptors = context.RequestServices.GetServices<IRouteInterceptor>();

            try
            {
                //matching interceptors could be cached by URL
                var matchingInterceptors = interceptors
                    .Where(interceptor => interceptor.Matches(context.GetRouteData(), HttpMethods.Get))
                    .ToArray();

                foreach (var subscriber in matchingInterceptors.OfType<ISubscribeToCompositionEvents>())
                {
                    subscriber.Subscribe(vm);
                }

                foreach (var appender in matchingInterceptors.OfType<IViewModelAppender>())
                {
                    pending.Add
                    (
                        appender.Append(vm, routeData, context.Request.Query)
                    );
                }

                if (pending.Count == 0)
                {
                    return (null, StatusCodes.Status404NotFound);
                }
                else
                {
                    await Task.WhenAll(pending);

                    return (vm, StatusCodes.Status200OK);
                }
            }
            finally
            {
                vm.CleanupSubscribers();
            }
        }
    }
}
