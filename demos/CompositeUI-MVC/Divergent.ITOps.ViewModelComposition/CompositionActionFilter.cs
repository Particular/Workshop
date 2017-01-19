using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Divergent.ITOps.ViewModelComposition
{
    public class CompositionActionFilter : IResultFilter
    {
        IEnumerable<IViewModelAppender> appenders;

        public CompositionActionFilter(IEnumerable<IViewModelAppender> appenders)
        {
            this.appenders = appenders;
        }

        public void OnResultExecuting(ResultExecutingContext filterContext)
        {
            dynamic vm = new ExpandoObject();

            var pending = new List<Task>();
            foreach (var appender in appenders.Where(a => a.Matches(filterContext.RouteData)))
            {
                var task = appender.Append(filterContext.RouteData, vm);
                pending.Add(task);
            }

            if (pending.Any())
            {
                Task.WhenAll(pending).GetAwaiter().GetResult();
                filterContext.Controller.ViewData.Model = vm;
            }
        }

        public void OnResultExecuted(ResultExecutedContext filterContext)
        {
            //NOP
        }
    }
}