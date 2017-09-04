using System.Collections.Generic;

namespace Divergent.Sales.Messages.Commands
{
    public class SubmitOrderCommand
    {
        public int CustomerId { get; set; }

        public List<int> Products { get; set; }
    }
}
