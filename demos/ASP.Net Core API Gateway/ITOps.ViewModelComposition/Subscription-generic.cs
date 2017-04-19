using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System;
using System.Threading.Tasks;

namespace ITOps.ViewModelComposition
{
    class Subscription<T> : Subscription
    {
        private Func<dynamic, T, RouteData, IQueryCollection, Task> subscription;

        public Subscription(Func<dynamic, T, RouteData, IQueryCollection, Task> subscription)
        {
            this.subscription = subscription;
        }

        public override Task Invoke(dynamic viewModel, object @event, RouteData routeData, IQueryCollection query) => subscription(viewModel, (T)@event, routeData, query);
    }
}
