using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Divergent.Shipping.Data.Migrations;
using Divergent.Shipping.Data.Models;

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
