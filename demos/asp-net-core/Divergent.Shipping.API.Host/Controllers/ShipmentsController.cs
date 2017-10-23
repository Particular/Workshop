using Divergent.Shipping.Data.Context;
using Divergent.Shipping.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Divergent.Shipping.API.Host.Controllers
{
    [RoutePrefix("api/shipments")]
    public class ShipmentsController : ApiController
    {
        [HttpGet]
        [Route("order/{orderNumber}")]
        public dynamic Order(int orderNumber)
        {
            using (var db = new ShippingContext())
            {
                var shipment = db.Shipments
                    .Where(s => s.OrderNumber == orderNumber)
                    .SingleOrDefault();

                return new
                {
                    shipment.OrderNumber,
                    shipment.Courier,
                    shipment.Status
                };
            }
        }

        [HttpGet]
        [Route("orders")]
        public IEnumerable<dynamic> Orders(string orderNumbers)
        {
            using (var db = new ShippingContext())
            {
                var orderNumbersArray = orderNumbers.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).Select(s => int.Parse(s)).ToArray();
                var shipments = db.Shipments
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
}
