using Divergent.Sales.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace Divergent.Sales.Data.Context;

public class SalesContext : DbContext
{
    public SalesContext(DbContextOptions options) : base(options)
    {
    }

    public DbSet<Product> Products { get; set; }
    public DbSet<Order> Orders { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Order>()
            .HasMany(e => e.Items)
            .WithOne(i => i.Order)
            .IsRequired();

        base.OnModelCreating(modelBuilder);
    }
}