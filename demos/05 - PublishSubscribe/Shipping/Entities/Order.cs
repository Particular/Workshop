using System;

namespace Shipping.Entities
{
    class Order
    {
        public Guid OrderId { get; set; }
        public bool IsPaid { get; set; }
    }
}
