using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Divergent.Sales.Data.Models;

namespace Divergent.Sales.Data.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        internal static Guid thePhantomMenace = Guid.Parse("77158b05-437d-4aa7-baaa-05df0bc60f17");
        internal static Guid attackOfTheClones = Guid.Parse("a5f4fc6d-9eb7-41b7-ac93-e294cf2cc2fa");
        internal static Guid revengeOfTheSith = Guid.Parse("e534c697-9bf2-4f7f-bc2c-73ec8f2c9f8b");
        internal static Guid aNewHope = Guid.Parse("524d2015-1240-4f0c-85af-17826f8d1e45");
        internal static Guid theEmpireStrikesBack = Guid.Parse("099952ec-2ab5-4334-a0d5-20fdb63daadb");
        internal static Guid returnOfTheJedi = Guid.Parse("11cb4ecd-5383-4da2-b3d4-8fd2a4e2117f");
        internal static Guid theForceAwakens = Guid.Parse("5e449efa-3f48-45d2-82f0-22483d97516a");

        internal static Product theForceAwakensProduct = new Product { Id = theForceAwakens, Name = "Star Wars : The Force Awakens" };
        internal static Product aNewHopeProduct = new Product { Id = aNewHope, Name = "Star Wars : A New Hope" };

        public async Task<List<Order>> Orders()
        {
            return OrderSeedData();
        }

        internal static List<Product> Products()
        {
            return new List<Product>
            {
                new Product {Id = thePhantomMenace, Name = "Star Wars : The Phantom Menace"},
                new Product {Id = attackOfTheClones, Name = "Star Wars : Attack of the Clones"},
                new Product {Id = revengeOfTheSith, Name = "Star Wars : Revenge of the Sith"},
                aNewHopeProduct,
                new Product {Id = theEmpireStrikesBack, Name = "Star Wars : The Empire Strikes Back"},
                new Product {Id = returnOfTheJedi, Name = "Star Wars : Return of the Jedi"},
                theForceAwakensProduct
            };
        }

        internal static List<Order> OrderSeedData()
        {
            var particularId = Guid.Parse("be49443c-5e6d-4892-9b7b-7395add4a44b");
            var particularOrderId = Guid.Parse("6c3945c9-0b64-414a-9e5f-892de482ae2b");

            var nserviceBusId = Guid.Parse("24453089-e36d-41ff-b119-82ae57482c74");
            var nserivceBusOrderId = Guid.Parse("41f74caf-2ef9-46ca-9445-2fcdf35751fb");

            // 'orderlines' for particular order
            var particularItems = new[]
            {
                new Item
                {
                    Id = Guid.Parse("c7db2d57-fad3-4a29-a34d-ddf6c4901028"),
                    Product = theForceAwakensProduct
                },
                new Item
                {
                    Id = Guid.Parse("e7163f34-f9a8-45cd-85fe-8187b451efca"),
                    Product = aNewHopeProduct
                }
            };

            // 'orderline' for nservicebus order
            var nservicebusItems = new[]
            {
                new Item
                {
                    Id = Guid.Parse("82c97550-e722-4773-ac51-eb1d341f2e0e"),
                    Product = theForceAwakensProduct
                }
            };

            return new List<Order>
            {
                new Order { CustomerId = particularId, DateTimeUtc = new DateTime(2016, 01, 01), State = "Payment Succeeded", Id = particularOrderId, Items = particularItems},
                new Order { CustomerId = nserviceBusId, DateTimeUtc = new DateTime(2016, 01, 02), State = "Payment awaiting", Id = nserivceBusOrderId, Items = nservicebusItems }
            };
        }
    }
}
