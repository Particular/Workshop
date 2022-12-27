namespace Divergent.Customers.Data.Models;

public class Order
{
    public int Id { get; set; }
    public Customer Customer { get; set; }
    public int CustomerId { get; set; }
    public int OrderId { get; set; }
}