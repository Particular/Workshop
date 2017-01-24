using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Routing;

namespace Divergent.ITOps.ViewModelComposition
{
    public interface ISubscribeToCompositionEvents: IRouteInterceptor
    {
        void Subscribe(ISubscriptionStorage subscriptionStorage);
    }
}
