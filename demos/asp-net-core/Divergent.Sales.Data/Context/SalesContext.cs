using Divergent.Sales.Data.Migrations;
using Divergent.Sales.Data.Models;
using System.Data.Entity;

namespace Divergent.Sales.Data.Context
{
    [DbConfigurationType(typeof(SQLiteConfig))]
    public class SalesContext : DbContext
    {
        public SalesContext() : base("Divergent.Sales")
        {
        }

        public IDbSet<Order> Orders { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            Database.SetInitializer(new DatabaseInitializer(modelBuilder));

            modelBuilder.Entity<Order>()
                .HasMany(order => order.Items)
                .WithRequired()
                .HasForeignKey(item => item.OrderId);

            base.OnModelCreating(modelBuilder);
        }
    }
}
