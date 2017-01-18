using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;

namespace Divergent.Frontend
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());

            //these should come from a DI container.
            var appenders = new List<ITOps.IViewModelAppender>()
            {
                new ITOps.Shipping.OrderDetailsViewModelAppender(),
                new ITOps.Sales.OrderDetailsViewModelAppender()
            };

            filters.Add(new ITOps.CompositionActionFilter(appenders));
        }
    }
}
