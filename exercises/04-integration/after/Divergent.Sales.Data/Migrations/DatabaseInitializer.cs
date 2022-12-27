using System.Linq;
using Divergent.Sales.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace Divergent.Sales.Data.Migrations;

public static class DatabaseInitializer 
{
    public static void Initialize(SalesContext context)
    {
        context.Database.EnsureCreated();

        if (context.Products.Any())
        {
            return;
        }

        context.Products.AddRange(SeedData.Products().ToArray());

        context.Database.OpenConnection();
        try
        {
            context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT dbo.Products ON");
            context.SaveChanges();
            context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT dbo.Products OFF");
        }
        finally
        {
            context.Database.CloseConnection();
        }

        context.Orders.AddRange(SeedData.Orders().ToArray());

        context.Database.OpenConnection();
        try
        {
            context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT dbo.Orders ON");
            context.SaveChanges();
            context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT dbo.Orders OFF");
        }
        finally
        {
            context.Database.CloseConnection();
        }
    }
}