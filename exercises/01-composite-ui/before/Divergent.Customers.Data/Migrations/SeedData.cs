using System.Collections.Generic;
using Divergent.Customers.Data.Models;

namespace Divergent.Customers.Data.Migrations
{
    internal static class SeedData
    {
        internal static List<Customer> Customers()
        {
            var particularId = 1;
            var particularOrderId = 1;

            var nserviceBusId = 2;
            var nserivceBusOrderId = 2;

            var acmeId = 3;

            return new List<Customer>
            {
                    new Customer
                    {
                        Id = particularId,
                        Name = "Particular Software ",
                        Orders = new List<Order>
                        {
                            new Order { CustomerId = particularId, OrderId = particularOrderId }
                        }
                    },
                    new Customer
                    {
                        Id = nserviceBusId,
                        Name = "NServiceBus Ltd.",
                        Orders = new List<Order>
                        {
                            new Order { CustomerId = nserviceBusId, OrderId = nserivceBusOrderId }
                        }
                    },
                    new Customer { Id = acmeId, Name = "Acme Inc.", Orders = new List<Order>() }
            };
        }
    }
}
