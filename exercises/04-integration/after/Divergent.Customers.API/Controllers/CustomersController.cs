using Divergent.Customers.Data.Context;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Divergent.Customers.API.Controllers;

[Route("api/customers")]
[ApiController]
public class CustomersController : ControllerBase
{
    private readonly CustomersContext _db;

    public CustomersController(CustomersContext db) => _db = db;

    [HttpGet("byorders")]
    public IEnumerable<dynamic> ByOrders(string orderIds)
    {
        var orderIdList = orderIds?.Split(',')
            .Select(id => int.Parse(id))
            .ToList() ?? new List<int>();

        var customers = _db.Customers
            .Include(customer => customer.Orders)
            .Where(customer => customer.Orders.Any(order => orderIdList.Contains(order.OrderId)))
            .ToList();

        return customers
            .SelectMany(customer => customer.Orders)
            .Where(order => orderIdList.Contains(order.OrderId))
            .Select(order => new
            {
                order.OrderId,
                CustomerName = customers.Single(customer => customer.Id == order.CustomerId).Name,
            })
            .ToList();
    }
}