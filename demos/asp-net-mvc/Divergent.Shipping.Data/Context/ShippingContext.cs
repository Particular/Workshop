using Divergent.Shipping.Data.Migrations;
using Divergent.Shipping.Data.Models;
using System.Data.Entity;

namespace Divergent.Shipping.Data.Context
{
    [DbConfigurationType(typeof(SqLiteConfig))]
    public class ShippingContext : DbContext
    {
        public ShippingContext() : base("Divergent.Shipping")
        {
        }

        public IDbSet<ShippingInfo> ShippingInfos { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            Database.SetInitializer(new DatabaseInitializer(modelBuilder));

            modelBuilder.Entity<ShippingInfo>();

            base.OnModelCreating(modelBuilder);
        }
    }
}
