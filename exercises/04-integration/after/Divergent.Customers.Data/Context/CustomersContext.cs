using Divergent.Customers.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace Divergent.Customers.Data.Context;

public class CustomersContext : DbContext
{
    public CustomersContext(DbContextOptions<CustomersContext> options) : base(options)
    {
    }

    public DbSet<Customer> Customers { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Customer>()
            .HasMany(e => e.Orders)
            .WithOne(k => k.Customer)
            .IsRequired();

        base.OnModelCreating(modelBuilder);
    }
}