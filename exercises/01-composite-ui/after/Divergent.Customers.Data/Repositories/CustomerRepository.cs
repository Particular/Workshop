using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Divergent.Customers.Data.Models;

namespace Divergent.Customers.Data.Repositories
{
    public class CustomerRepository : ICustomerRepository
    {
        public async Task<List<Customer>> Customers()
        {
            return SeedCustomers();
        }

        public async Task<Customer> Customer(Guid id)
        {
            return SeedCustomers().First(s => s.Id == id);
        }

        private List<Customer> SeedCustomers()
        {
            return new List<Customer>
            {
                new Customer { Id = Guid.Parse("be49443c-5e6d-4892-9b7b-7395add4a44b"), Name = "Particular Software "},
                new Customer { Id = Guid.Parse("24453089-e36d-41ff-b119-82ae57482c74"), Name = "NServiceBus Ltd." },
                new Customer { Id = Guid.Parse("bc0dc12b-1b94-4cf3-bd76-2679fd7fccae"), Name = "Acme Inc." }
            };
        }
    }
}
