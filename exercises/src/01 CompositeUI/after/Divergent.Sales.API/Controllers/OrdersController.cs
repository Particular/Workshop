using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Http;
using Divergent.Sales.Data.Context;

namespace Divergent.Sales.API.Controllers
{
    [RoutePrefix("api/orders")]
    public class OrdersController : ApiController
    {
        private readonly ISalesContext _context;

        public OrdersController(ISalesContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IEnumerable<dynamic> Get(int p = 0, int s = 10)
        {
            var orders = _context.Orders
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
                    ProductIds = o.Items.Select(i => i.Product.Id),
                    ItemsCount = o.Items.Count
                });
        }
    }
}
