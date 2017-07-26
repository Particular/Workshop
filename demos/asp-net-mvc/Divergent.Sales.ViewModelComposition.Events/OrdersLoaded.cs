using Divergent.ITOps.ViewModelComposition;
using System.Collections.Generic;

namespace Divergent.Sales.ViewModelComposition.Events
{
    public class OrdersLoaded : ICompositionEvent
    {
        public IDictionary<dynamic, dynamic> OrdersViewModel { get; set; }
    }
}
