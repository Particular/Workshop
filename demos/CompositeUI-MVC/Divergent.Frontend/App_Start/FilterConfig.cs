using Divergent.ITOps.ViewModelComposition;
using Divergent.Sales.ViewModelComposition;
using Divergent.Shipping.ViewModelComposition;
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
            var appenders = new List<IViewModelAppender>()
            {
                new SalesOrderDetailsViewModelAppender(),
                new ShippingOrderDetailsViewModelAppender()
            };

            filters.Add(new CompositionActionFilter(appenders));
        }
    }
}
