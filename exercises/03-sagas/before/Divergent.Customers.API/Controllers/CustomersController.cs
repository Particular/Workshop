using Divergent.Customers.Data.Context;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Http;

namespace Divergent.Customers.API.Controllers
{
    [RoutePrefix("api/customers")]
    public class CustomersController : ApiController
    {
        [HttpGet, Route("byorders")]
        public IEnumerable<dynamic> ByOrders(string orderIds)
        {
            var orderIdList = orderIds.Split(',')
                .Select(id => int.Parse(id))
                .ToList();

            List<Data.Models.Customer> customers;

            using (var db = new CustomersContext())
            {
                customers = db.Customers
                    .Include(customer => customer.Orders)
                    .Where(customer => customer.Orders.Any(order => orderIdList.Contains(order.OrderId)))
                    .ToList();
            }

            return customers
                .SelectMany(customer => customer.Orders)
                .Where(order => orderIdList.Contains(order.OrderId))
                .Select(order => new
                {
                    order.OrderId,
                    CustomerName = customers.Single(customer => customer.Id == order.CustomerId).Name,
                })
                .ToList();
        }
    }
}