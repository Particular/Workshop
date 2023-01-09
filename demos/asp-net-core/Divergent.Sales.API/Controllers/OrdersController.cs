using Divergent.Sales.Data.Models;
using ITOps.Data;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Divergent.Sales.API.Controllers
{
    [Route("api/orders")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly ILiteDbContext dbContext;

        public OrdersController(ILiteDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        [HttpGet("{orderNumber}")]
        public dynamic Get(int orderNumber)
        {
            var collection = dbContext.Database.GetCollection<Order>();
            return collection.Query()
                    .Include(order => order.Items)
                    .Where(order => order.OrderNumber == orderNumber)
                    .Select(order => new
                    {
                        OrderNumber = order.OrderNumber,
                        ItemsCount = order.Items.Count,
                    })
                    .SingleOrDefault();
        }

        [HttpGet]
        public IEnumerable<dynamic> Get(int pageIndex, int pageSize)
        {
            var collection = dbContext.Database.GetCollection<Order>();
            return collection.Query()
                .Include(order => order.Items)
                .ToList();
        }
    }
}
