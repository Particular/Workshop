using System.Collections.Specialized;
using System.Threading.Tasks;
using System.Web.Routing;

namespace Divergent.ITOps.ViewModelComposition
{
    public interface IViewModelAppender : IRouteInterceptor
    {
        Task Append(RequestContext request, dynamic viewModel);
    }
}
