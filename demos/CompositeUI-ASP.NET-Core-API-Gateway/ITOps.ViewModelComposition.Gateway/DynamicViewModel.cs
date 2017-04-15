using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITOps.ViewModelComposition.Gateway
{
    abstract class Subscription
    {
        public abstract Task Invoke(dynamic viewModel, object @event, RouteData routeData, IQueryCollection query);
    }

    class Subscription<T> : Subscription
    {
        private Func<dynamic, T, RouteData, IQueryCollection, Task> subscription;

        public Subscription(Func<dynamic, T, RouteData, IQueryCollection, Task> subscription)
        {
            this.subscription = subscription;
        }

        public override Task Invoke(dynamic viewModel, object @event, RouteData routeData, IQueryCollection query)
        {
            return subscription(viewModel, (T)@event, routeData, query);
        }
    }

    class DynamicViewModel : DynamicObject, ISubscriptionStorage, IViewModel
    {
        RouteData routeData;
        IQueryCollection query;
        IDictionary<Type, IList<Subscription>> subscriptions = new Dictionary<Type, IList<Subscription>>();
        IDictionary<string, object> properties = new Dictionary<string, object>();

        public DynamicViewModel(HttpContext context)
        {
            this.routeData = context.GetRouteData();
            this.query = context.Request.Query;
        }

        internal void CleanupSubscribers()
        {
            subscriptions.Clear();
        }

        public void Subscribe<T>(Func<dynamic, T, RouteData, IQueryCollection, Task> subscription)
        {
            if (!subscriptions.TryGetValue(typeof(T), out IList<Subscription> subscribers))
            {
                subscribers = new List<Subscription>();
                subscriptions.Add(typeof(T), subscribers);
            }

            subscribers.Add(new Subscription<T>(subscription));
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            return properties.TryGetValue(binder.Name, out result);
        }

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
            return base.GetDynamicMemberNames()
                .Union(properties.Keys)
                .Union(new[] { "RaiseEventAsync" });
        }

        public Task RaiseEventAsync(object @event)
        {
            if (subscriptions.TryGetValue(@event.GetType(), out IList<Subscription> subscribers))
            {
                var tasks = new List<Task>();
                foreach (var subscriber in subscribers)
                {
                    tasks.Add(subscriber.Invoke(this, @event, routeData, query));
                }

                return Task.WhenAll(tasks);
            }

            return Task.CompletedTask;
        }
    }
}
