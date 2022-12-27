using Divergent.Finance.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace Divergent.Finance.Data.Context;

public class FinanceContext : DbContext
{
    public FinanceContext(DbContextOptions options) : base(options)
    {
    }

    public DbSet<Price> Prices { get; set; }
    public DbSet<OrderItemPrice> OrderItemPrices { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Price>();
        modelBuilder.Entity<OrderItemPrice>();

        base.OnModelCreating(modelBuilder);
    }
}