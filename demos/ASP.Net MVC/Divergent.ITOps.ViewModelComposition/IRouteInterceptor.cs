using System.Web.Routing;

namespace Divergent.ITOps.ViewModelComposition
{
    public interface IRouteInterceptor
    {
        bool Matches(RequestContext request);
    }
}
