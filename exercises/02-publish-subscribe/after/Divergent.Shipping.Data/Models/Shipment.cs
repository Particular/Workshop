using System;

namespace Divergent.Shipping.Data.Models
{
    public class Shipment
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public bool IsOrderSubmitted { get; set; }
        public bool IsPaymentProcessed { get; set; }
        public bool IsShippedYet { get; set; }
    }
}
