using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ITOps.ViewModelComposition
{
    public interface IViewModel
    {
        Task RaiseEventAsync(object @event);
    }
}
