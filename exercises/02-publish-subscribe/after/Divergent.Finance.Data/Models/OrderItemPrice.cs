namespace Divergent.Finance.Data.Models
{
    public class OrderItemPrice
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public int ProductId { get; set; }
        public double ItemPrice { get; set; }
    }
}
