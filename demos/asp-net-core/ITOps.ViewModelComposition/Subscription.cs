using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System;
using System.Threading.Tasks;

namespace ITOps.ViewModelComposition
{
    class Subscription<T> : ISubscription
    {
        private Func<dynamic, T, RouteData, IQueryCollection, Task> handle;

        public Subscription(Func<dynamic, T, RouteData, IQueryCollection, Task> handle)
        {
            this.handle = handle;
        }

        public Task Invoke(dynamic viewModel, object @event, RouteData routeData, IQueryCollection query) => handle(viewModel, (T)@event, routeData, query);
    }
}
