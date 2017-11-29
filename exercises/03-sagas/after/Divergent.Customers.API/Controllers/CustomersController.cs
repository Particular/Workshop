using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using Divergent.Customers.Data.Context;
using System.Data.Entity;

namespace Divergent.Customers.API.Controllers
{
    [RoutePrefix("api/customers")]
    public class CustomersController : ApiController
    {
        [HttpGet, Route("byorders")]
        public IEnumerable<dynamic> ByOrders(string orderIds)
        {
            var _orderIds = orderIds.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries)
                .Select(id => int.Parse(id))
                .ToList();

            using (var db = new CustomersContext())
            {
                var query = db.Customers
                    .Include(c => c.Orders)
                    .Where(c => c.Orders.Any(o => _orderIds.Contains(o.OrderId)));

                var customers = query.ToList();
                var orders = customers.SelectMany(c => c.Orders).Where(o => _orderIds.Contains(o.OrderId));

                var results = new List<dynamic>();
                foreach (var order in orders)
                {
                    results.Add(new
                    {
                        OrderId = order.OrderId,
                        CustomerName = customers.Single(c => c.Id == order.CustomerId).Name
                    });
                }
                return results.ToArray();
            }
        }
    }
}
