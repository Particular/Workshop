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
        [Route("order/{id}")]
        public ShippingInfo Order(int id)
        {
            using (var db = new ShippingContext())
            {
                var info = db.ShippingInfos
                    .Where(si => si.OrderId == id)
                    .SingleOrDefault();

                return info;
            }
        }

        [HttpGet]
        [Route("orders")]
        public IEnumerable<ShippingInfo> Orders(string ids)
        {
            using (var db = new ShippingContext())
            {
                var idArray = ids.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).Select(s=>int.Parse(s)).ToArray();
                var info = db.ShippingInfos
                    .Where(si => idArray.Any(id => id == si.OrderId))
                    .ToArray();

                return info;
            }
        }
    }
}
