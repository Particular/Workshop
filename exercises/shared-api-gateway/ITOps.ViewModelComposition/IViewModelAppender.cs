using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System.Threading.Tasks;

namespace ITOps.ViewModelComposition
{
    public interface IViewModelAppender : IRouteInterceptor
    {
        Task Append(dynamic viewModel, RouteData routeData, IQueryCollection query);
    }
}
