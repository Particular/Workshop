using Divergent.Sales.Data.Migrations;
using Divergent.Sales.Data.Models;
using System.Data.Entity;

namespace Divergent.Sales.Data.Context
{
    [DbConfigurationType(typeof(SqLiteConfig))]
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
                .HasMany(e => e.Items)
                .WithRequired()
                .HasForeignKey(k => k.OrderId);

            base.OnModelCreating(modelBuilder);
        }
    }
}
