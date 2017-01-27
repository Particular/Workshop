using Divergent.Finance.Data.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;

namespace Finance.API.Controllers
{
    [RoutePrefix("api/prices")]
    public class PricingController : ApiController
    {
        [HttpGet, Route("orders/total")]
        public IDictionary<Guid, double> GetOrdersTotal(string orderIds)
        {
            var _orderIds = orderIds.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries)
                .Select(id => Guid.Parse(id))
                .ToList();

            using (var db = new FinanceContext())
            {
                var query = db.OrderItemPrices
                    .Where(op => _orderIds.Contains(op.OrderId))
                    .GroupBy(op => op.OrderId)
                    .Select(g => new
                    {
                        OrderId = g.Key,
                        Amount = g.Sum(op => op.ItemPrice)
                    });

                var result = query.ToDictionary(a => a.OrderId, a => a.Amount);
                return result;
            }
        }
    }
}
