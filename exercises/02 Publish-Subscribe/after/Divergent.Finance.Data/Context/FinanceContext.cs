using Divergent.Finance.Data.Migrations;
using Divergent.Finance.Data.Models;
using System.Data.Entity;

namespace Divergent.Finance.Data.Context
{
    public class FinanceContext : DbContext
    {
        public FinanceContext() : base("Divergent.Finance")
        {
        }

        public IDbSet<Price> Prices { get; set; }
        public IDbSet<OrderItemPrice> OrderItemPrices { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            Database.SetInitializer(new DatabaseInitializer());

            modelBuilder.Entity<Price>();
            modelBuilder.Entity<OrderItemPrice>();

            base.OnModelCreating(modelBuilder);
        }
    }
}
