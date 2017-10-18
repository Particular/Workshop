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
                new ShippingInfo() { OrderNumber = 1, Courier = "FedEx", Status = "Delivered"},
                new ShippingInfo() { OrderNumber = 2, Courier = "UPS", Status = "Shipment Pending"},
                new ShippingInfo() { OrderNumber = 3, Courier = "FedEx", Status = "Shipped"}
            };
        }
    }
}
