using Divergent.Sales.Data.Context;
using System.Data.Entity;
using System.Data.Entity.Migrations;

namespace Divergent.Sales.Data.Migrations
{
    public class DatabaseInitializer : CreateDatabaseIfNotExists<SalesContext>
    {
        protected override void Seed(SalesContext context)
        {
            context.Products.AddOrUpdate(k => k.Id, SeedData.Products().ToArray());

            context.Orders.AddOrUpdate(k => k.Id, SeedData.Orders().ToArray());
        }
    }
}