using Divergent.Sales.Data.Models;
using ITOps.EndpointConfig;
using NServiceBus;
using Microsoft.AspNetCore.Mvc;

namespace Divergent.Sales.API.Controllers;

[Route("api/orders")]
[ApiController]
public class OrdersController : ControllerBase
{
    private readonly ILiteDbContext db;

    public OrdersController(ILiteDbContext db)
    {
        this.db = db;
    }

    [HttpGet("")]
    public IEnumerable<dynamic> Get()
    {
        var col = db.Database.GetCollection<Order>();
        
        return col.Query()
            .Select(order => new
            {
                order.Id,
                order.CustomerId,
                ProductIds = order.Items,
                ItemsCount = order.Items.Count
            })
            .ToList();
    }
}