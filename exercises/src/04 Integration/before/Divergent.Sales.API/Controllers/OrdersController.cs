using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using Castle.Core.Internal;
using Divergent.Sales.Data.Context;
using NServiceBus;
using Divergent.Sales.Messages.Commands;

namespace Divergent.Sales.API.Controllers
{
    [RoutePrefix("api/orders")]
    public class OrdersController : ApiController
    {
        private readonly IEndpointInstance _endpoint;

        public OrdersController(IEndpointInstance endpoint)
        {
            _endpoint = endpoint;
        }

        [HttpPost, Route("createOrder")]
        public async Task<dynamic> CreateOrder(dynamic payload)
        {
            var customerId = int.Parse((String)payload.customerId);
            var productIds = ((IEnumerable<dynamic>)payload.products)
                .Select(p => int.Parse((String)p.productId))
                .ToList();

            await _endpoint.Send(new SubmitOrderCommand()
            {
                CustomerId = customerId,
                Products = productIds
            });

            return payload;
        }

        [HttpGet]
        public IEnumerable<dynamic> Get(int p = 0, int s = 10)
        {
            using (var _context = new SalesContext())
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
}
