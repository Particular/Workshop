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

namespace Sales.API.Controllers
{
    [RoutePrefix("api/orders")]
    public class OrdersController : ApiController
    {
        private readonly ISalesContext _context;
        private readonly IEndpointInstance _endpoint;

        public OrdersController(ISalesContext context, IEndpointInstance endpoint)
        {
            _context = context;
            _endpoint = endpoint;
        }

        [HttpPost, Route("createOrder")]
        public async Task<dynamic> CreateOrder(dynamic payload)
        {
            var newOrderId = Guid.NewGuid();
            var customerId = Guid.Parse((String)payload.customerId);
            var productIds = ((IEnumerable<dynamic>)payload.products)
                .Select(p => Guid.Parse((String)p.productId))
                .ToList();

            await _endpoint.Send(new SubmitOrderCommand()
            {
                CustomerId = customerId,
                OrderId = newOrderId,
                Products = productIds
            });

            return new
            {
                Id = newOrderId,
                CustomerId = customerId,
                ProductIds = productIds,
                ItemsCount = productIds.Count
            };
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
