using Divergent.Shipping.Data.Models;
using System;
using System.Collections.Generic;

namespace Divergent.Shipping.Data.Migrations
{
    internal static class SeedData
    {
        internal static List<ShippingInfo> ShippingInfos()
        {
            return new List<ShippingInfo>()
            {
                new ShippingInfo() { OrderId = 1, Courier = "FedEx", Status = "Delivered"}
            };
        }
    }
}
