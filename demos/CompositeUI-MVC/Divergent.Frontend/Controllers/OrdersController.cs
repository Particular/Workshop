using Divergent.Frontend.ITOps.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Divergent.Frontend.Controllers
{
    public class OrdersController : Controller
    {
        public async Task<ActionResult> First()
        {
            //should come from the DI container as a dependency
            var builder = new OrderBuilder(new OrderCustomerInfoProvider() );

            var vm = await builder.BuildFirstOrderViewModel();

            return View(vm);
        }
    }
}