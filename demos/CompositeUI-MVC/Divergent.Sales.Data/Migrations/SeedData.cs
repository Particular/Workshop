using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Divergent.Sales.Data.Models;

namespace Divergent.Sales.Data.Migrations
{
    internal static class SeedData
    {
        internal static List<Order> Orders()
        {
            return new List<Order>()
            {
                new Order() { DateTimeUtc = new DateTime(2016, 01, 01), Items = new []
                {
                    new Item()
                    {
                        ProductId = 1
                    },
                    new Item()
                    {
                        ProductId = 2
                    }
                }},
            };
        }

    }
}
