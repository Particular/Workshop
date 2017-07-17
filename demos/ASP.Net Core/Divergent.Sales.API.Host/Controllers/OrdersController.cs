using Divergent.Sales.Data.Context;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Http;

namespace Divergent.Sales.API.Host.Controllers
{
    [RoutePrefix("api/orders")]
    public class OrdersController : ApiController
    {
        [HttpGet]
        public dynamic Get(int id)
        {
            using (var db = new SalesContext())
            {
                var order = db.Orders
                    .Include(o => o.Items)
                    .Where(o => o.Id == id)
                    .Select(o => new
                    {
                        Number = o.Id,
                        o.Id,
                        ItemsCount = o.Items.Count
                    })
                    .SingleOrDefault();

                return order;
            }
        }

        [HttpGet]
        public IEnumerable<dynamic> Get(int pageIndex, int pageSize)
        {
            using (var db = new SalesContext())
            {
                var orders = db.Orders
                    .Include(o => o.Items)
                    .OrderBy(o => o.Id) //required by SQLite EF
                    .Skip(pageSize * pageIndex)
                    .Take(pageSize)
                    .Select(o => new
                    {
                        Number = o.Id,
                        o.Id,
                        ItemsCount = o.Items.Count
                    })
                    .ToArray();

                return orders;
            }
        }
    }
}
