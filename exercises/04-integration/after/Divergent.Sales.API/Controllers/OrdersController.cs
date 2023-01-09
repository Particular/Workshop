using Divergent.Sales.Data.Models;
using Divergent.Sales.Messages.Commands;
using ITOps.EndpointConfig;
using NServiceBus;
using Microsoft.AspNetCore.Mvc;

namespace Divergent.Sales.API.Controllers;

[Route("api/orders")]
[ApiController]
public class OrdersController : ControllerBase
{
    private readonly IMessageSession endpoint;
    private readonly ILiteDbContext db;

    public OrdersController(IMessageSession endpoint, ILiteDbContext db)
    {
        this.endpoint = endpoint;
        this.db = db;
    }

    [HttpPost("createOrder")]
    public async Task<dynamic> CreateOrder(dynamic payload)
    {
        var customerId = int.Parse((string)payload.customerId);
        var productIds = ((IEnumerable<dynamic>)payload.products)
            .Select(product => int.Parse((string)product.productId))
            .ToList();

        await endpoint.Send(new SubmitOrderCommand
        {
            CustomerId = customerId,
            Products = productIds
        });

        return payload;
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