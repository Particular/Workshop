using System.Threading.Tasks;

namespace Divergent.ITOps.ViewModelComposition
{
    abstract class Subscription
    {
        public abstract Task Invoke(dynamic viewModel, object @event, RequestContext requestContext);
    }
}
