using System.Linq;
using Divergent.Customers.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace Divergent.Customers.Data.Migrations;

public static class DatabaseInitializer
{
    public static void Initialize(CustomersContext context)
    {
        context.Database.EnsureCreated();

        if (context.Customers.Any())
        {
            return;
        }

        context.Customers.AddRange(SeedData.Customers().ToArray());
        
        context.Database.OpenConnection();
        try
        {
            context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT dbo.Customers ON");
            context.SaveChanges();
            context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT dbo.Customers OFF");
        }
        finally
        {
            context.Database.CloseConnection();
        }
    }
}