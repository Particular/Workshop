using System.Data.Entity;
using System.Data.Entity.Migrations;
using Divergent.Customers.Data.Context;

namespace Divergent.Customers.Data.Migrations
{
    public class DatabaseInitializer : CreateDatabaseIfNotExists<CustomersContext>
    {
        protected override void Seed(CustomersContext context)
        {
            context.Customers.AddOrUpdate(k => k.Id, SeedData.Customers().ToArray());
        }
    }
}
