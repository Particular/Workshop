using System.Data.Entity;
using Divergent.Customers.Data.Models;
using Divergent.Customers.Data.Migrations;

namespace Divergent.Customers.Data.Context
{
    public class CustomersContext : DbContext
    {
        public CustomersContext() : base("Divergent.Customers")
        {
        }

        public IDbSet<Customer> Customers { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            Database.SetInitializer(new DatabaseInitializer());

            modelBuilder.Entity<Customer>()
                .HasMany(e => e.Orders)
                .WithRequired()
                .HasForeignKey(k => k.CustomerId);

            base.OnModelCreating(modelBuilder);
        }
    }
}
