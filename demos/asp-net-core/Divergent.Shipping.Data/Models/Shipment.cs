namespace Divergent.Shipping.Data.Models
{
    public class Shipment
    {
        public int Id { get; set; } // required by EF

        public int OrderNumber { get; set; }

        public string Courier { get; set; }

        public string Status { get; set; }
    }
}
