using System.Collections.Generic;
using Divergent.Customers.Data.Models;

namespace Divergent.Customers.Data.Migrations;

internal static class SeedData
{
    internal static List<Customer> Customers()
    {
        return new List<Customer>
        {
            new Customer { Id = 1, Name = "Particular Software " },
            new Customer { Id = 2, Name = "NServiceBus Ltd." },
            new Customer { Id = 3, Name = "Acme Inc." }
        };
    }

    internal static List<Order> Orders()
    {
        return new List<Order>
        {
            new Order { CustomerId = 1, OrderId = 1},
            new Order { CustomerId = 2, OrderId = 2},
            new Order { CustomerId = 1, OrderId = 3}
        };
    }
}