using System.Data.Entity;
using System.Threading.Tasks;
using Divergent.Sales.Data.Migrations;
using Divergent.Sales.Data.Models;

namespace Divergent.Sales.Data.Context
{
    public interface ISalesContext
    {
        IDbSet<Product> Products { get; set; }
        IDbSet<Order> Orders { get; set; }

        Task<int> SaveChangesAsync();
    }

    public class SalesContext : DbContext, ISalesContext
    {
        public SalesContext() : base("Divergent.Sales")
        {
        }

        public IDbSet<Product> Products { get; set; }
        public IDbSet<Order> Orders { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            Database.SetInitializer(new DatabaseInitializer());

            modelBuilder.Entity<Order>()
                .HasMany(e => e.Items)
                .WithRequired()
                .HasForeignKey(k => k.OrderId);

            base.OnModelCreating(modelBuilder);
        }
    }
}
