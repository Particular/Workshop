using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Divergent.ITOps.ViewModelComposition
{
    public interface IViewModel
    {
        Task RaiseEventAsync(ICompositionEvent @event);
        //void OnDataRetrivalCompleted(Action<dynamic> handler);
    }
}
