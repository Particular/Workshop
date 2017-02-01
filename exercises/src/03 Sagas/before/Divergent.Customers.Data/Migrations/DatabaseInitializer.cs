using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using SQLite.CodeFirst;
using Divergent.Customers.Data.Context;

namespace Divergent.Customers.Data.Migrations
{
    public class DatabaseInitializer : SqliteDropCreateDatabaseWhenModelChanges<CustomersContext>
    {
        public DatabaseInitializer(DbModelBuilder modelBuilder) : base(modelBuilder)
        {
        }

        protected override void Seed(CustomersContext context)
        {
            context.Customers.AddOrUpdate(k => k.Id, SeedData.Customers().ToArray());
        }
    }
}
