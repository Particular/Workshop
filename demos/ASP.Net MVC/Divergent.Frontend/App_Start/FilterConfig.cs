using Castle.Windsor;
using System.Web.Mvc;

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
