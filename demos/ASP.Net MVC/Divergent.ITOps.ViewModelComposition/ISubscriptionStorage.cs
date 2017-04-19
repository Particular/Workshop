using System;
using System.Threading.Tasks;

namespace Divergent.ITOps.ViewModelComposition
{
    public interface ISubscriptionStorage
    {
        void Subscribe<T>(Func<dynamic, T, RequestContext, Task> subscription) where T : ICompositionEvent;
    }
}
