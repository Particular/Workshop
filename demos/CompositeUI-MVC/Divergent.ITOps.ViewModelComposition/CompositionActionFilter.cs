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
        IRouteFilter[] all;

        public CompositionActionFilter(IMessageBroker inMemoryBroker, IViewModelAppender[] appenders, ISubscribeCompositionEvents[] subscribers)
        {
            this.inMemoryBroker = inMemoryBroker;
            all = ((IRouteFilter[])appenders).Concat(subscribers).ToArray();
        }

        public void OnResultExecuting(ResultExecutingContext filterContext)
        {
            var vm = new DynamicViewModel(inMemoryBroker);
            try
            {
                var pending = new List<Task>();

                foreach (var interested in all.Where(a => a.Matches(filterContext.RouteData)))
                {
                    if (interested is ISubscribeCompositionEvents)
                    {
                        ((ISubscribeCompositionEvents)interested).Subscribe(vm);
                    }

                    if (interested is IViewModelAppender)
                    {
                        var task = ((IViewModelAppender)interested).Append(filterContext.RouteData, vm);
                        pending.Add(task);
                    }
                }

                if (pending.Any())
                {
                    Task.WhenAll(pending).GetAwaiter().GetResult();
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