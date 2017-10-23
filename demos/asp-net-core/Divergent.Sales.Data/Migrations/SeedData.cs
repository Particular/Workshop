using Divergent.Sales.Data.Models;
using System;

namespace Divergent.Sales.Data.Migrations
{
    internal static class SeedData
    {
        public static Order[] Orders { get; } = new[]
            {
                new Order()
                {
                    OrderNumber = 1,
                    DateTimeUtc = new DateTime(2016, 11, 01).ToUniversalTime(),
                    Items = new[]
                    {
                        new Item { ProductId = 1 },
                        new Item { ProductId = 2 }
                    }
                },
                new Order()
                {
                    OrderNumber = 2,
                    DateTimeUtc = new DateTime(2017, 01, 19).ToUniversalTime(),
                    Items = new[]
                    {
                        new Item { ProductId = 1 },
                        new Item { ProductId = 2 },
                        new Item { ProductId = 5 },
                        new Item { ProductId = 9 }
                    }
                },
                new Order()
                {
                    OrderNumber = 3,
                    DateTimeUtc = new DateTime(2017, 01, 19).ToUniversalTime(),
                    Items = new[]
                    {
                        new Item { ProductId = 1 },
                    }
                },
            };
    }
}
