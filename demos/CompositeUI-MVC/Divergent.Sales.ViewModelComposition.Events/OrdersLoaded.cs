using Divergent.ITOps.ViewModelComposition;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Divergent.Sales.ViewModelComposition.Events
{
    public class OrdersLoaded : ICompositionEvent
    {
        public IDictionary<dynamic, dynamic> Orders { get; set; }
    }
}
