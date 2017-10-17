using Divergent.Shipping.Data.Context;
using Divergent.Shipping.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Divergent.Shipping.API.Host.Controllers
{
    [RoutePrefix("api/shippinginfo")]
    public class ShippingInfoController : ApiController
    {
        [HttpGet]
        [Route("order/{orderNumber}")]
        public dynamic Order(int orderNumber)
        {
            using (var db = new ShippingContext())
            {
                var info = db.ShippingInfos
                    .Where(si => si.OrderNumber == orderNumber)
                    .SingleOrDefault();

                return new
                {
                    info.OrderNumber,
                    info.Courier,
                    info.Status
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
                var infos = db.ShippingInfos
                    .Where(si => orderNumbersArray.Any(id => id == si.OrderNumber))
                    .Select(si => new
                    {
                        si.OrderNumber,
                        si.Courier,
                        si.Status
                    })
                    .ToArray();

                return infos;
            }
        }
    }
}
