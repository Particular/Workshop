using Microsoft.AspNetCore.Routing;

namespace ITOps.ViewModelComposition
{
    public interface IRouteInterceptor
    {
        bool Matches(RouteData routeData, string httpMethod);
    }    
}
