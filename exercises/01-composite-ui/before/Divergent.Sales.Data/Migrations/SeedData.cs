using Divergent.Sales.Data.Models;
using System;
using System.Collections.Generic;

namespace Divergent.Sales.Data.Migrations
{
    internal static class SeedData
    {
        static int thePhantomMenace = 7;
        static int attackOfTheClones = 1;
        static int revengeOfTheSith = 2;
        static int aNewHope = 3;
        static int theEmpireStrikesBack = 4;
        static int returnOfTheJedi = 5;
        static int theForceAwakens = 6;

        static Product theForceAwakensProduct = new Product { Id = theForceAwakens, Name = "Star Wars : The Force Awakens" };
        static Product aNewHopeProduct = new Product { Id = aNewHope, Name = "Star Wars : A New Hope" };

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

        internal static List<Order> Orders()
        {
            var particularId = 1;
            var particularOrderId = 1;

            var nserviceBusId = 2;
            var nserivceBusOrderId = 2;

            // 'orderlines' for particular order
            var particularItems = new[]
            {
                new Item
                {
                    Product = theForceAwakensProduct
                },
                new Item
                {
                    Product = aNewHopeProduct
                }
            };

            // 'orderline' for nservicebus order
            var nservicebusItems = new[]
            {
                new Item
                {
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