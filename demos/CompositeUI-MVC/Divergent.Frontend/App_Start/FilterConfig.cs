using Divergent.ITOps.ViewModelComposition;
using Divergent.Sales.ViewModelComposition;
using Divergent.Shipping.ViewModelComposition;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using Castle.Windsor;
using System;

namespace Divergent.Frontend
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters, IWindsorContainer container)
        {
            filters.Add(new HandleErrorAttribute());

            var resultsFilter = container.ResolveAll<IResultFilter>();
            foreach (var item in resultsFilter)
            {
                filters.Add(item);
            }
        }
    }
}
