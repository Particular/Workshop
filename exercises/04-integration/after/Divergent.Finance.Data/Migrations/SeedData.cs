using Divergent.Finance.Data.Models;
using System.Collections.Generic;

namespace Divergent.Finance.Data.Migrations
{
    internal static class SeedData
    {
        public static List<Price> Prices()
        {
            var thePhantomMenace = 7;
            var attackOfTheClones = 1;
            var revengeOfTheSith = 2;
            var aNewHope = 3;
            var theEmpireStrikesBack = 4;
            var returnOfTheJedi = 5;
            var theForceAwakens = 6;

            return new List<Price>
            {
                new Price { ProductId = thePhantomMenace, ItemPrice = 10 },
                new Price { ProductId = attackOfTheClones, ItemPrice = 10 },
                new Price { ProductId = revengeOfTheSith, ItemPrice = 10 },
                new Price { ProductId = aNewHope, ItemPrice = 15 },
                new Price { ProductId = theEmpireStrikesBack, ItemPrice = 15 },
                new Price { ProductId = returnOfTheJedi, ItemPrice = 15 },
                new Price { ProductId = theForceAwakens, ItemPrice = 25 }
            };
        }

        public static List<OrderItemPrice> OrderItemPrices()
        {
            var particularOrderId = 1;
            var nserivceBusOrderId = 2;

            return new List<OrderItemPrice>
            {
                new OrderItemPrice { OrderId = particularOrderId, ItemPrice = 10 },
                new OrderItemPrice { OrderId = particularOrderId, ItemPrice = 10 },
                new OrderItemPrice { OrderId = particularOrderId, ItemPrice = 10 },
                new OrderItemPrice { OrderId = particularOrderId,  ItemPrice = 15 },
                new OrderItemPrice { OrderId = nserivceBusOrderId, ItemPrice = 15 },
                new OrderItemPrice { OrderId = nserivceBusOrderId, ItemPrice = 15 },
                new OrderItemPrice { OrderId = nserivceBusOrderId, ItemPrice = 25 }
            };
        }
    }
}
