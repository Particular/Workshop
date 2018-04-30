using System.Data.Entity;

namespace Divergent.Customers.ITOps
{
    internal class CustomersContext : DbContext
    {
        public CustomersContext() : base("Divergent.Customers")
        {
        }

        public IDbSet<Customer> Customers { get; set; }
    }
}
