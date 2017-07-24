using System;
using System.Collections.Generic;

namespace Divergent.Sales.Messages.Commands
{
    public class SubmitOrderCommand
    {
        public Guid CustomerId { get; set; }

        public Guid OrderId { get; set; }
        /// <summary>
        
        /// List of products the customer ordered
        /// </summary>
        public List<Guid> Products { get; set; }
    }
}
