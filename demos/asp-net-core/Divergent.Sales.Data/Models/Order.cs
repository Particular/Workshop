using System;
using System.Collections.Generic;

namespace Divergent.Sales.Data.Models
{
    public class Order
    {
        public int Id { get; set; } // required by EF

        public int OrderNumber { get; set; }

        public DateTime DateTimeUtc { get; set; }

        public ICollection<Item> Items { get; set; }
    }

    public class Item
    {
        public int Id { get; set; } // required by EF

        public int OrderId { get; set; } // required by EF

        public int ProductId { get; set; }
    }
}
