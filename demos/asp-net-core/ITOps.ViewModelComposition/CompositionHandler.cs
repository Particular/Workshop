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
            var routeData = context.GetRouteData();

            var viewModel = new DynamicViewModel(routeData, context.Request.Query);

            // matching interceptors could be cached by URL
            var interceptors = context.RequestServices.GetServices<IRouteInterceptor>()
                .Where(interceptor => interceptor.Matches(routeData, HttpMethods.Get))
                .ToList();

            try
            {
                foreach (var subscriber in interceptors.OfType<ISubscribeToCompositionEvents>())
                {
                    subscriber.Subscribe(viewModel);
                }

                var pendingTasks = new List<Task>();

                foreach (var appender in interceptors.OfType<IViewModelAppender>())
                {
                    pendingTasks.Add(appender.Append(viewModel, routeData, context.Request.Query));
                }

                if (!pendingTasks.Any())
                {
                    return (null, StatusCodes.Status404NotFound);
                }
                else
                {
                    await Task.WhenAll(pendingTasks);

                    return (viewModel, StatusCodes.Status200OK);
                }
            }
            finally
            {
                viewModel.ClearSubscriptions();
            }
        }
    }
}
