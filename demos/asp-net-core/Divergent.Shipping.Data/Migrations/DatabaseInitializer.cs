using System;
using Divergent.Shipping.Data.Models;
using LiteDB;

namespace Divergent.Shipping.Data.Migrations;

public static class DatabaseInitializer
{
    public static void Initialize(LiteDatabase context)
    {
        if (context == null) throw new ArgumentNullException(nameof(context));

        var shipments = context.GetCollection<Shipment>();
        if (shipments.Count() > 0)
            return;

        shipments.InsertBulk(SeedData.Shipments);
    }
}