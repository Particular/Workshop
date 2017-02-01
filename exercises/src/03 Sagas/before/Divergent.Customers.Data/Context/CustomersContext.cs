using System.Data.Entity;
using Divergent.Customers.Data.Models;
using Divergent.Customers.Data.Migrations;
using System.Configuration;

namespace Divergent.Customers.Data.Context
{
    [DbConfigurationType(typeof(SqLiteConfig))]
    public class CustomersContext : DbContext
    {
        public CustomersContext() : base("Divergent.Customers")
        {
        }

        public IDbSet<Customer> Customers { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            var runMigrations = ConfigurationManager.AppSettings["SQLite/execute/migrations"];
            if (!string.IsNullOrWhiteSpace(runMigrations) && runMigrations.ToLower() == "true")
            {
                Database.SetInitializer(new DatabaseInitializer(modelBuilder));
            }

            modelBuilder.Entity<Customer>()
                .HasMany(e => e.Orders)
                .WithRequired()
                .HasForeignKey(k => k.CustomerId);

            base.OnModelCreating(modelBuilder);
        }
    }
}
