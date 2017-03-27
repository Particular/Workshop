using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Topics.Radical.ComponentModel.Messaging;

namespace Divergent.ITOps.ViewModelComposition
{
    public class CompositionActionFilter : IResultFilter
    {
        IMessageBroker inMemoryBroker;
        IRouteInterceptor[] routeInterceptors;

        public CompositionActionFilter(IMessageBroker inMemoryBroker, IRouteInterceptor[] routeInterceptors)
        {
            this.inMemoryBroker = inMemoryBroker;
            this.routeInterceptors = routeInterceptors;
        }

        public void OnResultExecuting(ResultExecutingContext filterContext)
        {
            var requestInfo = new RequestContext(filterContext.RouteData, filterContext.HttpContext.Request.QueryString);
            var vm = new DynamicViewModel(inMemoryBroker, requestInfo);

            try
            {
                var pending = new List<Task>();

                foreach (var interceptor in routeInterceptors.Where(a => a.Matches(requestInfo)))
                {
                    var subscriber = interceptor as ISubscribeToCompositionEvents;
                    if (subscriber != null)
                    {
                        subscriber.Subscribe(vm);
                    }

                    var appender = interceptor as IViewModelAppender;
                    if (appender != null)
                    {
                        var task = appender.Append(requestInfo, vm);
                        pending.Add(task);
                    }
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