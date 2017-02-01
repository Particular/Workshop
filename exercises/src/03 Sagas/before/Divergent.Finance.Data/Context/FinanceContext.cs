using Divergent.Finance.Data.Migrations;
using Divergent.Finance.Data.Models;
using System.Configuration;
using System.Data.Entity;

namespace Divergent.Finance.Data.Context
{
    [DbConfigurationType(typeof(SqLiteConfig))]
    public class FinanceContext : DbContext
    {
        public FinanceContext() : base("Divergent.Finance")
        {
        }

        public IDbSet<Price> Prices { get; set; }
        public IDbSet<OrderItemPrice> OrderItemPrices { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            var runMigrations = ConfigurationManager.AppSettings["SQLite/execute/migrations"];
            if (!string.IsNullOrWhiteSpace(runMigrations) && runMigrations.ToLower() == "true")
            {
                Database.SetInitializer(new DatabaseInitializer(modelBuilder));
            }

            modelBuilder.Entity<Price>();
            modelBuilder.Entity<OrderItemPrice>();

            base.OnModelCreating(modelBuilder);
        }
    }
}
