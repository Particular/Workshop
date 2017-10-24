using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Threading.Tasks;

namespace ITOps.ViewModelComposition
{
    class DynamicViewModel : DynamicObject, IPublishCompositionEvents
    {
        RouteData routeData;
        IQueryCollection query;
        IDictionary<Type, IList<EventHandler<object>>> subscriptions = new Dictionary<Type, IList<EventHandler<object>>>();
        IDictionary<string, object> properties = new Dictionary<string, object>();

        public DynamicViewModel(HttpContext context)
        {
            this.routeData = context.GetRouteData();
            this.query = context.Request.Query;
        }

        public void CleanupSubscribers() => subscriptions.Clear();

        public void Subscribe<TEvent>(EventHandler<TEvent> handler)
        {
            if (!subscriptions.TryGetValue(typeof(TEvent), out var handlers))
            {
                handlers = new List<EventHandler<object>>();
                subscriptions.Add(typeof(TEvent), handlers);
            }

            handlers.Add((pageViewModel, @event, routeData, query) => handler(pageViewModel, (TEvent)@event, routeData, query));
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result) => properties.TryGetValue(binder.Name, out result);

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            properties[binder.Name] = value;
            return true;
        }

        public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
        {
            result = null;

            if (binder.Name == "RaiseEventAsync")
            {
                result = this.RaiseEventAsync(args[0]);
                return true;
            }

            return false;
        }

        public override IEnumerable<string> GetDynamicMemberNames()
        {
            foreach (var item in properties.Keys)
            {
                yield return item;
            }

            yield return "RaiseEventAsync";
        }

        public Task RaiseEventAsync(object @event)
        {
            if (subscriptions.TryGetValue(@event.GetType(), out var handlers))
            {
                var tasks = new List<Task>();
                foreach (var handler in handlers)
                {
                    tasks.Add(handler.Invoke(this, @event, routeData, query));
                }

                return Task.WhenAll(tasks);
            }

            return Task.CompletedTask;
        }
    }
}
