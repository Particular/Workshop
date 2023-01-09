using Divergent.Shipping.Data.Models;
using ITOps.Data;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace Divergent.Shipping.API.Controllers
{
    [Route("api/shipments")]
    [ApiController]
    public class ShipmentsController : ControllerBase
    {
        private readonly ILiteDbContext dbContext;

        public ShipmentsController(ILiteDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        [HttpGet("order/{orderNumber}")]
        public dynamic Order(int orderNumber)
        {
            var collection = dbContext.Database.GetCollection<Shipment>();
            var shipment = collection.Query()
                                .Where(s => s.OrderNumber == orderNumber)
                                .Select(s => new { s.OrderNumber, s.Courier, s.Status })
                                .SingleOrDefault();

            return new
            {
                shipment.OrderNumber,
                shipment.Courier,
                shipment.Status
            };
        }

        [HttpGet("orders")]
        public IEnumerable<dynamic> Orders(string orderNumbers)
        {
            var orderNumbersArray = orderNumbers.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).Select(s => int.Parse(s)).ToArray();

            var collection = dbContext.Database.GetCollection<Shipment>();
            var shipments = collection.Query()
                .Where(s => orderNumbersArray.Any(id => id == s.OrderNumber))
                .Select(s => new
                {
                    s.OrderNumber,
                    s.Courier,
                    s.Status
                })
                .ToArray();

            return shipments;
        }
    }
}
