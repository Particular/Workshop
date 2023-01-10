using System;
using Divergent.Sales.Data.Models;
using LiteDB;

namespace Divergent.Sales.Data.Migrations;

public static class DatabaseInitializer
{
    public static void Initialize(LiteDatabase context)
    {
        if (context == null) throw new ArgumentNullException(nameof(context));

        var shipments = context.GetCollection<Order>();
        if (shipments.Count() > 0)
            return;

        shipments.InsertBulk(SeedData.Orders);
    }
}