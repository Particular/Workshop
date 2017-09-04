using System;

namespace Divergent.Customers.Data.Models
{
    public class CustomerOrderRelationship
    {
        public Guid CustomerId { get; set; }
        public Guid OrderId { get; set; }
    }
}
