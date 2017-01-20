using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Topics.Radical.ComponentModel.Messaging;

namespace Divergent.ITOps.ViewModelComposition
{
    class DynamicViewModel : DynamicObject, ISubscriptionStorage
    {
        IMessageBroker inMemoryBroker;

        public DynamicViewModel( IMessageBroker inMemoryBroker )
        {
            this.inMemoryBroker = inMemoryBroker;
        }

        Dictionary<string, object> properties = new Dictionary<string, object>();

        internal void CleanupSubscribers()
        {
            inMemoryBroker.Unsubscribe(this);
        }

        public void Subscribe<T>(Action<dynamic, T> subscription) where T : ICompositionEvent
        {
            inMemoryBroker.Subscribe<T>(this, subscription);
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
                inMemoryBroker.BroadcastAsync(this, args[0]).GetAwaiter().GetResult();
                return true;
            }

            return false;
        }
    }
}
