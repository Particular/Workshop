using System.Collections.Specialized;
using System.Web.Routing;

namespace Divergent.ITOps.ViewModelComposition
{
    public class RequestContext
    {
        public RequestContext(RouteData routeData, NameValueCollection quesryString)
        {
            this.RouteData = routeData;
            this.QueryString = quesryString;
        }

        public RouteData RouteData { get; private set; }
        public NameValueCollection QueryString { get; private set; }
    }
}
