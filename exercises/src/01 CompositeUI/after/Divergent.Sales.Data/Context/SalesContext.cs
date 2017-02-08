using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Divergent.Sales.Data.Migrations;
using Divergent.Sales.Data.Models;
using System.Configuration;

namespace Divergent.Sales.Data.Context
{
    [DbConfigurationType(typeof(SqLiteConfig))]
    public class SalesContext : DbContext
    {
        public SalesContext() : base("Divergent.Sales")
        {
        }

        public IDbSet<Product> Products { get; set; }
        public IDbSet<Order> Orders { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            var runMigrations = ConfigurationManager.AppSettings["SQLite/execute/migrations"];
            if (!string.IsNullOrWhiteSpace(runMigrations) && runMigrations.ToLower() == "true")
            {
                Database.SetInitializer(new DatabaseInitializer(modelBuilder));
            }

            modelBuilder.Entity<Order>()
                .HasMany(e => e.Items)
                .WithRequired()
                .HasForeignKey(k => k.OrderId);

            base.OnModelCreating(modelBuilder);
        }
    }
}
