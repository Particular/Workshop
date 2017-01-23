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

        public CompositionActionFilter(IMessageBroker inMemoryBroker, IViewModelAppender[] appenders, ISubscribeToCompositionEvents[] subscribers)
        {
            this.inMemoryBroker = inMemoryBroker;
            routeInterceptors = ((IRouteInterceptor[])appenders).Concat(subscribers).ToArray();
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
                    if (interceptor is ISubscribeToCompositionEvents)
                    {
                        ((ISubscribeToCompositionEvents)interceptor).Subscribe(vm);
                    }

                    if (interceptor is IViewModelAppender)
                    {
                        var task = ((IViewModelAppender)interceptor).Append(requestInfo, vm);
                        pending.Add(task);
                    }
                }

                if (pending.Any())
                {
                    Task.WhenAll(pending)
                        .GetAwaiter()
                        .GetResult();

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