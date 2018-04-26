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
        [HttpGet, Route]
        public IEnumerable<dynamic> Get()
        {
            using (var db = new SalesContext())
            {
                return db.Orders
                    .Include(order => order.Items)
                    .Include(order => order.Items.Select(item => item.Product))
                    .Select(order => new
                    {
                        order.Id,
                        order.CustomerId,
                        ProductIds = order.Items.Select(item => item.Product.Id).ToList(),
                        ItemsCount = order.Items.Count,
                    })
                    .ToList();
            }
        }
    }
}
