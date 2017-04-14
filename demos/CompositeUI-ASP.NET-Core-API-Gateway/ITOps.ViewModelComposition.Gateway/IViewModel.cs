using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ITOps.ViewModelComposition.Gateway
{
    public interface IViewModel
    {
        Task RaiseEventAsync(object @event);
    }
}
