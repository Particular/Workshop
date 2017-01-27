using Divergent.Finance.Data.Models;
using System;
using System.Collections.Generic;

namespace Divergent.Finance.Data.Migrations
{
    internal static class SeedData
    {
        public static List<Price> Prices()
        {
            var thePhantomMenace = Guid.Parse("77158b05-437d-4aa7-baaa-05df0bc60f17");
            var attackOfTheClones = Guid.Parse("a5f4fc6d-9eb7-41b7-ac93-e294cf2cc2fa");
            var revengeOfTheSith = Guid.Parse("e534c697-9bf2-4f7f-bc2c-73ec8f2c9f8b");
            var aNewHope = Guid.Parse("524d2015-1240-4f0c-85af-17826f8d1e45");
            var theEmpireStrikesBack = Guid.Parse("099952ec-2ab5-4334-a0d5-20fdb63daadb");
            var returnOfTheJedi = Guid.Parse("11cb4ecd-5383-4da2-b3d4-8fd2a4e2117f");
            var theForceAwakens = Guid.Parse("5e449efa-3f48-45d2-82f0-22483d97516a");

            return new List<Price>()
            {
                new Price() { ProductId = thePhantomMenace, ItemPrice = 10 },
                new Price() { ProductId = attackOfTheClones, ItemPrice = 10 },
                new Price() { ProductId = revengeOfTheSith, ItemPrice = 10 },
                new Price() { ProductId = aNewHope, ItemPrice = 15 },
                new Price() { ProductId = theEmpireStrikesBack, ItemPrice = 15 },
                new Price() { ProductId = returnOfTheJedi, ItemPrice = 15 },
                new Price() { ProductId = theForceAwakens, ItemPrice = 25 }
            };
        }

        public static List<OrderItemPrice> OrderItemPrices()
        {
            var particularOrderId = Guid.Parse("6c3945c9-0b64-414a-9e5f-892de482ae2b");
            var nserivceBusOrderId = Guid.Parse("41f74caf-2ef9-46ca-9445-2fcdf35751fb");

            return new List<OrderItemPrice>()
            {
                new OrderItemPrice() { OrderId = particularOrderId, ItemPrice = 10 },
                new OrderItemPrice() { OrderId = particularOrderId, ItemPrice = 10 },
                new OrderItemPrice() { OrderId = particularOrderId, ItemPrice = 10 },
                new OrderItemPrice() { OrderId = particularOrderId,  ItemPrice = 15 },
                new OrderItemPrice() { OrderId = nserivceBusOrderId, ItemPrice = 15 },
                new OrderItemPrice() { OrderId = nserivceBusOrderId, ItemPrice = 15 },
                new OrderItemPrice() { OrderId = nserivceBusOrderId, ItemPrice = 25 }
            };
        }
    }
}
