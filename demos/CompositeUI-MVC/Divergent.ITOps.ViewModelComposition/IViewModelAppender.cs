using System.Threading.Tasks;
using System.Web.Routing;

namespace Divergent.ITOps.ViewModelComposition
{
    public interface IViewModelAppender: IRouteFilter
    {
        Task Append(RouteData routeData, dynamic viewModel);
    }
}
