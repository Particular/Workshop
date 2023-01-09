using System;
using Divergent.Finance.Data.Models;
using LiteDB;

namespace Divergent.Finance.Data.Migrations;

public static class DatabaseInitializer
{
    public static void Initialize(LiteDatabase context)
    {
        if (context == null) throw new ArgumentNullException(nameof(context));

        var prices = context.GetCollection<Price>();
        if (prices.Count() > 0)
            return;

        prices.InsertBulk(SeedData.Prices());

        var orderItemPrices = context.GetCollection<OrderItemPrice>();
        orderItemPrices.InsertBulk(SeedData.OrderItemPrices());
    }
}