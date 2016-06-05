using System;
using System.Collections.Generic;

namespace Divergent.Sales.Data.Models
{
    public class Order
    {
        public Guid Id { get; set; }

        public Guid CustomerId { get; set; }

        public DateTime DateTimeUtc { get; set; }

        public string State { get; set; }

        public ICollection<Item> Items { get; set; }
    }

    public class Item
    {
        public Guid Id { get; set; }
        public Guid OrderId { get; set; }
        public Product Product { get; set; }
    }
}
