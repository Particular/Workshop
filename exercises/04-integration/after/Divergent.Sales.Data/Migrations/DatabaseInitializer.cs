using System;
using Divergent.Sales.Data.Models;
using LiteDB;

namespace Divergent.Sales.Data.Migrations;

public static class DatabaseInitializer 
{
    public static void Initialize(LiteDatabase context)
    {
        if (context == null) throw new ArgumentNullException(nameof(context));
        
        var products = context.GetCollection<Product>();
        if (products.Count() > 0)
            return;

        products.InsertBulk(SeedData.Products());

        var orders = context.GetCollection<Order>();
        orders.InsertBulk(SeedData.Orders());
    }
}