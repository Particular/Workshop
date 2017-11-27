using Divergent.Sales.Data.Context;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Http;

namespace Divergent.Sales.API.Controllers
{
    [RoutePrefix("api/orders")]
    public class OrdersController : ApiController
    {
        [HttpGet]
        public IEnumerable<dynamic> Get()
        {
            using (var _context = new SalesContext())
            {
                var orders = _context.Orders
                    .Include(i => i.Items)
                    .Include(i => i.Items.Select(x => x.Product))
                    .ToArray();

                return orders
                    .Select(o => new
                    {
                        o.Id,
                        o.CustomerId,
                        ProductIds = o.Items.Select(i => i.Product.Id),
                    });
            }
        }
    }
}
