using System;
using System.Threading.Tasks;

namespace Divergent.ITOps.ViewModelComposition
{
    class Subscription<T> : Subscription
    {
        Func<dynamic, T, RequestContext, Task> subscription;

        public Subscription(Func<dynamic, T, RequestContext, Task> subscription)
        {
            this.subscription = subscription;
        }

        public override Task Invoke(dynamic viewModel, object @event, RequestContext requestContext) => subscription(viewModel, (T)@event, requestContext);
    }
}
