using Divergent.Sales.Data.Context;
using Divergent.Sales.Messages.Commands;
using NServiceBus;
using Microsoft.AspNetCore.Mvc;

namespace Divergent.Sales.API.Controllers;

[Route("api/orders")]
[ApiController]
public class OrdersController : ControllerBase
{
    private readonly IMessageSession _endpoint;
    private readonly SalesContext _db;

    public OrdersController(IMessageSession endpoint, SalesContext db)
    {
        _endpoint = endpoint;
        _db = db;
    }

    [HttpPost("createOrder")]
    public async Task<dynamic> CreateOrder(dynamic payload)
    {
        var customerId = int.Parse((string)payload.customerId);
        var productIds = ((IEnumerable<dynamic>)payload.products)
            .Select(product => int.Parse((string)product.productId))
            .ToList();

        await _endpoint.Send(new SubmitOrderCommand
        {
            CustomerId = customerId,
            Products = productIds
        });

        return payload;
    }

    [HttpGet("")]
    public IEnumerable<dynamic> Get()
    {
        return _db.Orders
            .Select(order => new
            {
                order.Id,
                order.CustomerId,
                ProductIds = order.Items.Select(item => item.Product.Id).ToList(),
                ItemsCount = order.Items.Count
            })
            .ToList();
    }
}