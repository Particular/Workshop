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
                new Order()
                {
                    Id = 1,
                    DateTimeUtc = new DateTime(2016, 11, 01),
                    Items = new []
                    {
                        new Item()
                        {
                            ProductId = 1
                        },
                        new Item()
                        {
                            ProductId = 2
                        }
                    }
                },
                new Order()
                {
                    Id = 2,
                    DateTimeUtc = new DateTime(2017, 01, 19),
                    Items = new []
                    {
                        new Item()
                        {
                            ProductId = 1
                        },
                        new Item()
                        {
                            ProductId = 2
                        },
                        new Item()
                        {
                            ProductId = 5
                        },
                        new Item()
                        {
                            ProductId = 9
                        }
                    }
                },
                new Order()
                {
                    Id = 3,
                    DateTimeUtc = new DateTime(2017, 01, 19),
                    Items = new []
                    {
                        new Item()
                        {
                            ProductId = 1
                        },
                    }
                },
            };
        }

    }
}
