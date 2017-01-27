using System;

namespace Divergent.Finance.Data.Models
{
    public class OrderItemPrice
    {
        public Guid Id { get; set; }
        public Guid OrderId { get; set; }
        public Guid ProductId { get; set; }
        public double ItemPrice { get; set; }
    }
}
