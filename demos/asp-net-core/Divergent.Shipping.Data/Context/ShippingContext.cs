using Divergent.Shipping.Data.Migrations;
using Divergent.Shipping.Data.Models;
using System.Data.Entity;

namespace Divergent.Shipping.Data.Context
{
    [DbConfigurationType(typeof(SQLiteConfig))]
    public class ShippingContext : DbContext
    {
        public ShippingContext() : base("Divergent.Shipping")
        {
        }

        public IDbSet<Shipment> Shipments { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            Database.SetInitializer(new DatabaseInitializer(modelBuilder));

            modelBuilder.Entity<Shipment>();

            base.OnModelCreating(modelBuilder);
        }
    }
}
