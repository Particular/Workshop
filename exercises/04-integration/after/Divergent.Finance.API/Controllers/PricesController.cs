using Divergent.Finance.Data.Context;
using Microsoft.AspNetCore.Mvc;

namespace Divergent.Finance.API.Controllers;

[Route("api/prices")]
[ApiController]
public class PricingController : ControllerBase
{
    private readonly FinanceContext _db;

    public PricingController(FinanceContext db) => _db = db;

    [HttpGet("orders/total")]
    public IEnumerable<dynamic> GetOrdersTotal(string orderIds)
    {
        var orderIdList = orderIds?.Split(',')
            .Select(id => int.Parse(id))
            .ToList() ?? new List<int>();

        return _db.OrderItemPrices
            .Where(orderItemPrice => orderIdList.Contains(orderItemPrice.OrderId))
            .GroupBy(orderItemPrice => orderItemPrice.OrderId)
            .Select(orderGroup => new
            {
                OrderId = orderGroup.Key,
                Amount = orderGroup.Sum(orderItemPrice => orderItemPrice.ItemPrice),
            })
            .ToList();
    }
}