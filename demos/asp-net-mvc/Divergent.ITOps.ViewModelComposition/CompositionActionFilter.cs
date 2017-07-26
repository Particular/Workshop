using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Divergent.ITOps.ViewModelComposition
{
    public class CompositionActionFilter : IResultFilter
    {
        IRouteInterceptor[] routeInterceptors;

        public CompositionActionFilter(IRouteInterceptor[] routeInterceptors)
        {
            this.routeInterceptors = routeInterceptors;
        }

        public void OnResultExecuting(ResultExecutingContext filterContext)
        {
            var requestInfo = new RequestContext(filterContext.RouteData, filterContext.HttpContext.Request.QueryString);
            var vm = new DynamicViewModel(requestInfo);

            //can be cached by URL
            var interceptors = routeInterceptors
                .Where(a => a.Matches(requestInfo))
                .ToArray();

            try
            {
                var pending = new List<Task>();

                foreach (var subscriber in interceptors.OfType<ISubscribeToCompositionEvents>())
                {
                    subscriber.Subscribe(vm);
                }

                foreach (var appender in interceptors.OfType<IViewModelAppender>())
                {
                    pending.Add(appender.Append(requestInfo, vm));
                }

                if (pending.Count > 0)
                {
                    Task.WaitAll(pending.ToArray());

                    filterContext.Controller.ViewData.Model = vm;
                }
            }
            finally
            {
                vm.CleanupSubscribers();
            }
        }

        public void OnResultExecuted(ResultExecutedContext filterContext)
        {
            //NOP
        }
    }
}