using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Primitives;
using System.Collections.Generic;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace ITOps.ViewModelComposition
{
    public class CompositionHandler
    {
        public static async Task<(dynamic ViewModel, int StatusCode)> HandleGetRequest(HttpContext context)
        {
            var viewModel = new DynamicViewModel(context.GetRouteData(), context.Request.Query);
            var routeData = context.GetRouteData();

            // matching interceptors could be cached by URL
            var interceptors = context.RequestServices.GetServices<IRouteInterceptor>()
                .Where(interceptor => interceptor.Matches(context.GetRouteData(), HttpMethods.Get))
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
                    var statusCode = StatusCodes.Status200OK;

                    await Task.WhenAll(pendingTasks.ToArray())
                        .ContinueWith(t=> 
                        {
                            if (t.IsFaulted)
                            {
                                statusCode = StatusCodes.Status500InternalServerError;
                                var flattenedException = t.Exception.Flatten();
                                context.Response.Headers.Add("composition-errors", new StringValues(flattenedException.InnerExceptions.Select(exception => HtmlEncoder.Default.Encode(exception.ToString())).ToArray()));
                            }
                        });

                    return (viewModel, statusCode);
                }
            }
            finally
            {
                viewModel.ClearSubscriptions();
            }
        }
    }
}
