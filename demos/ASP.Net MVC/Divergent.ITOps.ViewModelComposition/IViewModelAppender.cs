using System.Threading.Tasks;

namespace Divergent.ITOps.ViewModelComposition
{
    public interface IViewModelAppender : IRouteInterceptor
    {
        Task Append(RequestContext request, dynamic viewModel);
    }
}
