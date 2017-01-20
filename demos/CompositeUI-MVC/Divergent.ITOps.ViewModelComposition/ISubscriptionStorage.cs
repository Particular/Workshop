using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Divergent.ITOps.ViewModelComposition
{
    public interface ISubscriptionStorage
    {
        void Subscribe<T>(Func<dynamic, T, Task> subscription) where T : ICompositionEvent;
    }
}
