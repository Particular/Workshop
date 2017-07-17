using Divergent.Sales.Data.Context;
using SQLite.CodeFirst;
using System.Data.Entity;
using System.Data.Entity.Migrations;

namespace Divergent.Sales.Data.Migrations
{
    public class DatabaseInitializer : SqliteCreateDatabaseIfNotExists<SalesContext>
    {
        public DatabaseInitializer(DbModelBuilder modelBuilder) : base(modelBuilder)
        {
        }

        protected override void Seed(SalesContext context)
        {
            context.Orders.AddOrUpdate(k => k.Id, SeedData.Orders().ToArray());
        }
    }
}
