using System;
using System.Collections.Generic;

namespace Divergent.Sales.Data.Models;

public class Order
{
    public int Id { get; set; }

    public int CustomerId { get; set; }

    public DateTime DateTimeUtc { get; set; }

    public string State { get; set; }

    public ICollection<int> Items { get; set; }
}