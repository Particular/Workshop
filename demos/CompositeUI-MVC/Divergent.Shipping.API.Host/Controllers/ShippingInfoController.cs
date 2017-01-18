using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Http;
using Divergent.Shipping.Data.Context;

namespace Divergent.Shipping.API.Host.Controllers
{
    [RoutePrefix("api/shippinginfo")]
    public class ShippingInfoController : ApiController
    {
        [HttpGet]
        [Route("order/{id}")]
        public dynamic Order(int id)
        {
            using (var db = new ShippingContext())
            {
                var info = db.ShippingInfos
                    .Where(si => si.OrderId == id)
                    .SingleOrDefault();

                return info;
            }
        }
    }
}
