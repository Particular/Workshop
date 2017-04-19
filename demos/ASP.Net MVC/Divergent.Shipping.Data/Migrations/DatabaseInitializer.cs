using Divergent.Shipping.Data.Context;
using SQLite.CodeFirst;
using System.Data.Entity;
using System.Data.Entity.Migrations;

namespace Divergent.Shipping.Data.Migrations
{
    public class DatabaseInitializer : SqliteCreateDatabaseIfNotExists<ShippingContext>
    {
        public DatabaseInitializer(DbModelBuilder modelBuilder) : base(modelBuilder)
        {
        }

        protected override void Seed(ShippingContext context)
        {
            context.ShippingInfos.AddOrUpdate(k => k.Id, SeedData.ShippingInfos().ToArray());
        }
    }
}
