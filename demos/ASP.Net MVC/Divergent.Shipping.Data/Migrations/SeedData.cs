using Divergent.Shipping.Data.Models;
using System.Collections.Generic;

namespace Divergent.Shipping.Data.Migrations
{
    internal static class SeedData
    {
        internal static List<ShippingInfo> ShippingInfos()
        {
            return new List<ShippingInfo>()
            {
                new ShippingInfo() { OrderId = 1, Courier = "FedEx", Status = "Delivered"},
                new ShippingInfo() { OrderId = 2, Courier = "UPS", Status = "Shipment Pending"},
                new ShippingInfo() { OrderId = 3, Courier = "FedEx", Status = "Shipped"}
            };
        }
    }
}
