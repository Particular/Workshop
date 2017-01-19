using System.Threading.Tasks;
using System.Web.Routing;

namespace Divergent.ITOps.ViewModelComposition
{
    public interface IViewModelAppender
    {
        Task Append(RouteData routeData, dynamic viewModel);
        bool Matches(RouteData routeData);
    }
}
