using System;
using System.Collections.Generic;

namespace Divergent.Sales.Messages.Events
{
    public class OrderSubmittedEvent
    {
        public Guid OrderId { get; set; }
        public Guid CustomerId { get; set; }

        public List<Guid> Products { get; set; }
    }
}
