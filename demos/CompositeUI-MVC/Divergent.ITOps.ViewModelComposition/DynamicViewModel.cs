using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;

namespace Divergent.ITOps.ViewModelComposition
{
    abstract class Subscription
    {
        public abstract Task Invoke(dynamic viewModel, object @event, RequestContext requestContext);
    }

    class Subscription<T> : Subscription
    {
        private Func<dynamic, T, RequestContext, Task> subscription;

        public Subscription(Func<dynamic, T, RequestContext, Task> subscription)
        {
            this.subscription = subscription;
        }

        public override Task Invoke(dynamic viewModel, object @event, RequestContext requestContext)
        {
            return subscription(viewModel, (T)@event, requestContext);
        }
    }


    class DynamicViewModel : DynamicObject, ISubscriptionStorage, IViewModel
    {
        RequestContext requestContext;
        IDictionary<Type, IList<Subscription>> subscriptions = new Dictionary<Type, IList<Subscription>>();
        IDictionary<string, object> properties = new Dictionary<string, object>();

        public DynamicViewModel(RequestContext requestContext)
        {
            this.requestContext = requestContext;
        }

        internal void CleanupSubscribers()
        {
            subscriptions.Clear();
        }

        public void Subscribe<T>(Func<dynamic, T, RequestContext, Task> subscription) where T : ICompositionEvent
        {
            IList<Subscription> subscribers;
            if (!subscriptions.TryGetValue(typeof(T), out subscribers))
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

            if (binder.Name == "RaiseEvent")
            {
                this.RaiseEventAsync((ICompositionEvent)args[0]).GetAwaiter().GetResult();
                return true;
            }
            else if (binder.Name == "RaiseEventAsync")
            {
                result = this.RaiseEventAsync((ICompositionEvent)args[0]);
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

            yield return "RaiseEvent";
            yield return "RaiseEventAsync";
        }

        public Task RaiseEventAsync(ICompositionEvent @event)
        {
            IList<Subscription> subscribers;
            if (subscriptions.TryGetValue(@event.GetType(), out subscribers))
            {
                var tasks = new List<Task>();
                foreach (var subscriber in subscribers)
                {
                    tasks.Add(subscriber.Invoke(this, @event, requestContext));
                }

                return Task.WhenAll(tasks);
            }

            return Task.CompletedTask;
        }
    }
}
