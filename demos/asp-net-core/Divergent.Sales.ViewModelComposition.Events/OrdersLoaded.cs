using System.Collections.Generic;

namespace Divergent.Sales.ViewModelComposition.Events
{
    public class OrdersLoaded
    {
        public IDictionary<dynamic, dynamic> OrderViewModelDictionary { get; set; }
    }
}
