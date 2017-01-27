using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Divergent.Customers.Data.Models;

namespace Divergent.Customers.Data.Migrations
{
    internal static class SeedData
    {
        internal static List<Customer> Customers()
        {
            var particularId = Guid.Parse("be49443c-5e6d-4892-9b7b-7395add4a44b");
            var particularOrderId = Guid.Parse("6c3945c9-0b64-414a-9e5f-892de482ae2b");

            var nserviceBusId = Guid.Parse("24453089-e36d-41ff-b119-82ae57482c74");
            var nserivceBusOrderId = Guid.Parse("41f74caf-2ef9-46ca-9445-2fcdf35751fb");

            var acmeId = Guid.Parse("bc0dc12b-1b94-4cf3-bd76-2679fd7fccae");

            return new List<Customer>()
            {
                    new Customer()
                    {
                        Id = particularId,
                        Name = "Particular Software ",
                        Orders = new List<Order>()
                        {
                            new Order() { CustomerId = particularId, OrderId = particularOrderId }
                        }
                    },
                    new Customer()
                    {
                        Id = nserviceBusId,
                        Name = "NServiceBus Ltd.",
                        Orders = new List<Order>()
                        {
                            new Order() { CustomerId = nserviceBusId, OrderId = nserivceBusOrderId }
                        }
                    },
                    new Customer() { Id = acmeId, Name = "Acme Inc." }
            };
        }
    }
}
