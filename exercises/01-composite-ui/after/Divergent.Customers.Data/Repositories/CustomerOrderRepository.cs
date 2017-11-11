using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Divergent.Customers.Data.Models;

namespace Divergent.Customers.Data.Repositories
{
    public class CustomerOrderRepository
    {
        public Task<List<CustomerOrderRelationship>> CustomerOrderRelationships()
        {
            return Task.FromResult(SeedCustomerOrderRelationship());
        }

        private List<CustomerOrderRelationship> SeedCustomerOrderRelationship()
        {
            var particularId = Guid.Parse("be49443c-5e6d-4892-9b7b-7395add4a44b");
            var particularOrderId = Guid.Parse("6c3945c9-0b64-414a-9e5f-892de482ae2b");

            var nserviceBusId = Guid.Parse("24453089-e36d-41ff-b119-82ae57482c74");
            var nserivceBusOrderId = Guid.Parse("41f74caf-2ef9-46ca-9445-2fcdf35751fb");

            return new List<CustomerOrderRelationship>
            {
                new CustomerOrderRelationship { CustomerId= particularId, OrderId = particularOrderId},
                new CustomerOrderRelationship { CustomerId= nserviceBusId, OrderId = nserivceBusOrderId},
            };
        }
    }
}
