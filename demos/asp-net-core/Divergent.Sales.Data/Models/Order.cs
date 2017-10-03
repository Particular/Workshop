using System;
using System.Collections.Generic;

namespace Divergent.Sales.Data.Models
{
    public class Order
    {
        public int Id { get; set; }

        public DateTime DateTimeUtc { get; set; }

        public ICollection<Item> Items { get; set; }
    }

    public class Item
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public int ProductId { get; set; }
    }
}
