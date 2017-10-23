namespace Divergent.Shipping.Data.Models
{
    public class Shipment
    {
        public int Id { get; set; }
        public int OrderNumber { get; internal set; }
        public string Courier { get; internal set; }
        public string Status { get; internal set; }
    }
}
