using Divergent.Customers.Data.Models;
using ITOps.EndpointConfig;
using Microsoft.AspNetCore.Mvc;

namespace Divergent.Customers.API.Controllers;

[Route("api/customers")]
[ApiController]
public class CustomersController : ControllerBase
{
    private readonly ILiteDbContext _db;

    public CustomersController(ILiteDbContext db) => _db = db;

    [HttpGet("byorders")]
    public IEnumerable<dynamic> ByOrders(string orderIds)
    {
        var orderIdList = orderIds?.Split(',')
            .Select(id => int.Parse(id))
            .ToList() ?? new List<int>();

        var customerCollection = _db.Database.GetCollection<Customer>();
        var orderCollection = _db.Database.GetCollection<Order>();

        var orders = orderCollection.Query()
            .Where(s => orderIdList.Contains(s.OrderId))
            .ToList();

        var customerIds = orders.GroupBy(s => s.CustomerId).Select(s => s.Key);

        var customers = customerCollection.Query()
            .Where(s => customerIds.Contains(s.Id))
            .ToList();

        return orders.Join(customers, o => o.CustomerId, c => c.Id,
            (order, customer) => new { order.OrderId, CustomerId = customer.Id, CustomerName = customer.Name });
    }
}