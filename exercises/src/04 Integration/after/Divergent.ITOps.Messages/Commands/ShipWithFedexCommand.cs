using System;
using System.Collections.Generic;

namespace Divergent.ITOps.Messages.Commands
{
    public class ShipWithFedexCommand
    {
        public Guid OrderId { get; set; }
        public Guid CustomerId { get; set; }
        public List<Guid> Products { get; set; }
    }
}
