using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using Divergent.Customers.Data.Models;
using Divergent.Customers.Data.Context;

namespace Customers.API.Controllers
{
    [RoutePrefix("api/customers")]
    public class CustomersController : ApiController
    {
        [HttpGet, Route("ByIds/{ids}")]
        public IEnumerable<Customer> ByIds(string ids)
        {
            using (var db = new CustomersContext())
            {
                var _ids = ids.Split("|".ToCharArray(), StringSplitOptions.RemoveEmptyEntries)
                    .Select(id => Guid.Parse(id))
                    .ToList();

                var query = db.Customers
                    .Where(c => _ids.Contains(c.Id));

                var customers = query.ToList();

                return customers;
            }
        }

        [HttpGet, Route("byorders")]
        public IDictionary<Guid, Customer> ByOrders(string orderIds)
        {
            var _orderIds = orderIds.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries)
                .Select(id => Guid.Parse(id))
                .ToList();

            using (var db = new CustomersContext())
            {
                var query = db.Customers
                    .Where(c => c.Orders.Any(o => _orderIds.Contains(o.OrderId)));

                var customers = query.ToList();
                var orders = customers.SelectMany(c => c.Orders).Where(o => _orderIds.Contains(o.OrderId));

                var result = new Dictionary<Guid, Customer>();
                foreach (var order in orders)
                {
                    result.Add(order.OrderId, customers.Single(c => c.Id == order.CustomerId));
                }
                return result;
            }
        }
    }
}
