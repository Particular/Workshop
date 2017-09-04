using System.Data.Entity;
using Divergent.Sales.Data.Migrations;
using Divergent.Sales.Data.Models;

namespace Divergent.Sales.Data.Context
{
    public class SalesContext : DbContext
    {
        public SalesContext() : base("Divergent.Sales")
        {
        }

        public IDbSet<Product> Products { get; set; }
        public IDbSet<Order> Orders { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            Database.SetInitializer(new DatabaseInitializer());

            modelBuilder.Entity<Order>()
                .HasMany(e => e.Items)
                .WithRequired()
                .HasForeignKey(k => k.OrderId);

            base.OnModelCreating(modelBuilder);
        }
    }
}
