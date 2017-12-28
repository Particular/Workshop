using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System.Threading.Tasks;

namespace ITOps.ViewModelComposition
{
    public delegate Task EventHandler<TEvent>(dynamic pageViewModel, TEvent @event, RouteData routeData, IQueryCollection query);
}
