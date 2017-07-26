using System.Collections.Generic;

namespace Divergent.ITOps.Messages.Commands
{
    public class ShipWithFedexCommand
    {
        public int OrderId { get; set; }
        public int CustomerId { get; set; }
        public List<int> Products { get; set; }
    }
}
