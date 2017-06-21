using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System.Threading.Tasks;

namespace ITOps.ViewModelComposition
{
    interface ISubscription
    {
        Task Invoke(dynamic viewModel, object @event, RouteData routeData, IQueryCollection query);
    }
}
