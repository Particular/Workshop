using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using SQLite.CodeFirst;
using Divergent.Finance.Data.Context;

namespace Divergent.Finance.Data.Migrations
{
    public class DatabaseInitializer : SqliteCreateDatabaseIfNotExists<FinanceContext>
    {
        public DatabaseInitializer(DbModelBuilder modelBuilder) : base(modelBuilder)
        {
        }

        protected override void Seed(FinanceContext context)
        {
            context.Prices.AddOrUpdate(k => k.Id, SeedData.Prices().ToArray());
            context.OrderItemPrices.AddOrUpdate(k => k.Id, SeedData.OrderItemPrices().ToArray());
        }
    }
}
