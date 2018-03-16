using Divergent.Finance.Data.Context;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Divergent.Finance.API.Controllers
{
    [RoutePrefix("api/prices")]
    public class PricingController : ApiController
    {
        [HttpGet, Route("orders/total")]
        public IEnumerable<dynamic> GetOrdersTotal(string orderIds)
        {
            var orderIdList = orderIds.Split(',')
                .Select(id => int.Parse(id))
                .ToList();

            using (var db = new FinanceContext())
            {
                return db.OrderItemPrices
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
    }
}
