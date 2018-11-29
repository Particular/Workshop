using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

using FastMember;

namespace ITOps.ViewModelComposition
{
    public class CompositionHandler
    {
        public static async Task<(dynamic ViewModel, int StatusCode)> HandleGetRequest(
            HttpContext context, 
            dynamic classicViewModel)
        {
            var routeData = context.GetRouteData();

            var viewModel = new DynamicViewModel(routeData, context.Request.Query);

            CopyProperties(viewModel, classicViewModel);

            // matching interceptors could be cached by URL
            var interceptors = context.RequestServices
                                      .GetServices<IRouteInterceptor>()
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

                    if (classicViewModel != null)
                    {
                        CopyProperties(classicViewModel, viewModel);
                        return (classicViewModel, StatusCodes.Status200OK);
                    }

                    return (viewModel, StatusCodes.Status200OK);
                }
            }
            finally
            {
                viewModel.ClearSubscriptions();
            }
        }

        private static void CopyProperties(dynamic targetViewModel, dynamic sourceViewModel)
        {
            if (sourceViewModel == null)
            {
                return;
            }

            var accessor = ObjectAccessor.Create(targetViewModel);

            var properties = TypeDescriptor.GetProperties(sourceViewModel);
            foreach (PropertyDescriptor property in properties)
            {
                var propertyName = property.Name;
                var propertyValue = property.GetValue(sourceViewModel);

                accessor[propertyName] = propertyValue;
            }
        }
    }
}