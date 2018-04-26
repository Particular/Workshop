using Divergent.Sales.Data.Context;
using Divergent.Sales.Messages.Commands;
using NServiceBus;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;

namespace Divergent.Sales.API.Controllers
{
    [RoutePrefix("api/orders")]
    public class OrdersController : ApiController
    {
        private readonly IEndpointInstance _endpoint;

        public OrdersController(IEndpointInstance endpoint) => _endpoint = endpoint;

        [HttpPost, Route("createOrder")]
        public async Task<dynamic> CreateOrder(dynamic payload)
        {
            var customerId = int.Parse((string)payload.customerId);
            var productIds = ((IEnumerable<dynamic>)payload.products)
                .Select(product => int.Parse((string)product.productId))
                .ToList();

            await _endpoint.Send(new SubmitOrderCommand
            {
                CustomerId = customerId,
                Products = productIds
            });

            return payload;
        }

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
                        ItemsCount = order.Items.Count
                    })
                    .ToList();
            }
        }
    }
}
