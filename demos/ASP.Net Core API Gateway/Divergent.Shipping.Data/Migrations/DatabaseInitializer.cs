using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using SQLite.CodeFirst;
using Divergent.Shipping.Data.Context;

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
