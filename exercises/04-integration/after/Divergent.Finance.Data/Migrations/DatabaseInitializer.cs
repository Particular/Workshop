using System.Linq;
using Divergent.Finance.Data.Context;

namespace Divergent.Finance.Data.Migrations;

public static class DatabaseInitializer 
{
    public static void Initialize(FinanceContext context)
    {
        context.Database.EnsureCreated();

        if (context.Prices.Any())
        {
            return;
        }

        context.Prices.AddRange(SeedData.Prices().ToArray());
            
        context.OrderItemPrices.AddRange(SeedData.OrderItemPrices().ToArray());

        context.SaveChanges();
    }
}