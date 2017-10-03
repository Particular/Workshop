using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace ITOps.ViewModelComposition
{
    public interface ISubscribeToCompositionEvents : IRouteInterceptor
    {
        void Subscribe(ISubscriptionStorage subscriptionStorage, RouteData routeData, IQueryCollection query);
    }
}
