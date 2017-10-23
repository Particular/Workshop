using Divergent.Shipping.Data.Models;

namespace Divergent.Shipping.Data.Migrations
{
    internal static class SeedData
    {
        public static Shipment[] Shipments { get; } = new[]
            {
                new Shipment { OrderNumber = 1, Courier = "FedEx", Status = "Delivered"},
                new Shipment { OrderNumber = 2, Courier = "UPS", Status = "Shipment Pending"},
                new Shipment { OrderNumber = 3, Courier = "FedEx", Status = "Shipped"},
            };
    }
}
