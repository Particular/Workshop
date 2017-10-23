using Divergent.Shipping.Data.Models;
using System.Collections.Generic;

namespace Divergent.Shipping.Data.Migrations
{
    internal static class SeedData
    {
        internal static List<Shipment> Shipments()
        {
            return new List<Shipment>()
            {
                new Shipment() { OrderNumber = 1, Courier = "FedEx", Status = "Delivered"},
                new Shipment() { OrderNumber = 2, Courier = "UPS", Status = "Shipment Pending"},
                new Shipment() { OrderNumber = 3, Courier = "FedEx", Status = "Shipped"}
            };
        }
    }
}
