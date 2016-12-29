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
        private readonly ISalesContext _context;

        public OrdersController(ISalesContext context)
        {
            _context = context;
        }

        [HttpGet]
        public dynamic First()
        {
            var orders = _context.Orders
                .Include(o=>o.Items)
                .ToArray();

            return orders
                .Select(o => new
                {
                    o.Id,
                    o.CustomerId,
                    ItemsCount = o.Items.Count
                })
                .First();
        }
    }
}
