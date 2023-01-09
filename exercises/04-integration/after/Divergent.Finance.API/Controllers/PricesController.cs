using Divergent.Finance.Data.Models;
using ITOps.EndpointConfig;
using Microsoft.AspNetCore.Mvc;

namespace Divergent.Finance.API.Controllers;

[Route("api/prices")]
[ApiController]
public class PricingController : ControllerBase
{
    private readonly ILiteDbContext db;

    public PricingController(ILiteDbContext db) => this.db = db;

    [HttpGet("orders/total")]
    public IEnumerable<dynamic> GetOrdersTotal(string orderIds)
    {
        var orderIdList = orderIds?.Split(',')
            .Select(int.Parse)
            .ToList() ?? new List<int>();

        return db.Database.GetCollection<OrderItemPrice>().Query()
            .Where(orderItemPrice => orderIdList.Contains(orderItemPrice.OrderId)).ToList()
            .GroupBy(orderItemPrice => orderItemPrice.OrderId)
            .Select(orderGroup => new
            {
                OrderId = orderGroup.Key,
                Amount = orderGroup.Sum(orderItemPrice => orderItemPrice.ItemPrice),
            });
    }
}