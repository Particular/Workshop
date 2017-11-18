using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using Divergent.Sales.Data.Repositories;

namespace Divergent.Sales.API.Controllers
{
    [RoutePrefix("api/orders")]
    public class OrdersController : ApiController
    {
        private readonly IOrderRepository _repository;

        public OrdersController(IOrderRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public async Task<IEnumerable<dynamic>> Get()
        {
            var orders = await _repository.Orders();

            return orders
                .Select(o => new
                {
                    o.Id,
                    o.CustomerId,
                    ProductIds = o.Items.Select(i => i.Product.Id),
                });
        }
    }
}
