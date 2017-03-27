using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Topics.Radical.ComponentModel.Messaging;

namespace Divergent.ITOps.ViewModelComposition
{
    class DynamicViewModel : DynamicObject, ISubscriptionStorage, IViewModel
    {
        //Action<dynamic> onDataRetrivalCompletedHandler = vm => { };
        IMessageBroker inMemoryBroker;
        RequestContext request;

        public DynamicViewModel(IMessageBroker inMemoryBroker, RequestContext request)
        {
            this.inMemoryBroker = inMemoryBroker;
            this.request = request;
        }

        Dictionary<string, object> properties = new Dictionary<string, object>();

        //public void OnDataRetrivalCompleted(Action<dynamic> handler)
        //{
        //    this.onDataRetrivalCompletedHandler = handler;
        //}

        //void RaiseOnDataRetrivalCompleted()
        //{
        //    if (onDataRetrivalCompletedHandler != null)
        //    {
        //        onDataRetrivalCompletedHandler(this);
        //    }
        //}

        internal void CleanupSubscribers()
        {
            inMemoryBroker.Unsubscribe(this);
        }

        public void Subscribe<T>(Func<dynamic, T, RequestContext, Task> subscription) where T : ICompositionEvent
        {
            inMemoryBroker.Subscribe<T>(this, (sender, @event) =>
            {
                return subscription(sender, @event, request);
            });
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

        public Task RaiseEventAsync(ICompositionEvent @event)
        {
            var task = inMemoryBroker.BroadcastAsync(this, @event);
            return task;
        }
    }
}
