using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Http;
using Divergent.Sales.Data.Context;

namespace Sales.API.Controllers
{
    [RoutePrefix("api/orders")]
    public class OrdersController : ApiController
    {
        [HttpGet]
        public IEnumerable<dynamic> Get(int p = 0, int s = 10)
        {
            using (var db = new SalesContext())
            {
                var orders = db.Orders
                    .Include(i => i.Items)
                    .Include(i => i.Items.Select(x => x.Product))
                    .ToArray();

                return orders
                    .Skip(p * s)
                    .Take(s)
                    .Select(o => new
                    {
                        o.Id,
                        o.CustomerId,
                        ProductIds = o.Items.Select(i => i.Product.Id)
                    });
            }
        }
    }
}
