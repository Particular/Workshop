using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Threading.Tasks;

namespace ITOps.ViewModelComposition
{
    internal class DynamicViewModel : DynamicObject, IPublishCompositionEvents
    {
        private readonly RouteData routeData;
        private readonly IQueryCollection query;

        private IDictionary<Type, List<EventHandler<object>>> subscriptions = new Dictionary<Type, List<EventHandler<object>>>();
        private IDictionary<string, object> properties = new Dictionary<string, object>();

        public DynamicViewModel(RouteData routeData, IQueryCollection query)
        {
            this.routeData = routeData;
            this.query = query;
        }

        public void Subscribe<TEvent>(EventHandler<TEvent> handler)
        {
            if (!subscriptions.TryGetValue(typeof(TEvent), out var handlers))
            {
                handlers = new List<EventHandler<object>>();
                subscriptions.Add(typeof(TEvent), handlers);
            }

            handlers.Add((pageViewModel, @event, routeData, query) => handler(pageViewModel, (TEvent)@event, routeData, query));
        }

        public void ClearSubscriptions() => subscriptions.Clear();

        public override bool TryGetMember(GetMemberBinder binder, out object result) => properties.TryGetValue(binder.Name, out result);

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            properties[binder.Name] = value;
            return true;
        }

        public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
        {
            result = null;

            if (binder.Name == nameof(RaiseEventAsync))
            {
                result = RaiseEventAsync(args[0]);
                return true;
            }

            return false;
        }

        public override IEnumerable<string> GetDynamicMemberNames()
        {
            foreach (var propertyName in properties.Keys)
            {
                yield return propertyName;
            }

            yield return nameof(RaiseEventAsync);
        }

        private Task RaiseEventAsync(object @event)
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
