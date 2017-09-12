using System.Data.Entity;
using Divergent.Shipping.Data.Models;

namespace Divergent.Shipping.Data.Context
{
    public class ShippingContext : DbContext
    {
        public ShippingContext() : base("Divergent.Shipping")
        {
        }

        public IDbSet<Shipment> Shipments { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Shipment>();

            base.OnModelCreating(modelBuilder);
        }
    }
}
