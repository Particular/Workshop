using ITOps.ViewModelComposition;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System.Net.Http;
using ITOps.ViewModelComposition.Json;

namespace Divergent.Sales.ViewModelComposition
{
    public class OrderDetailsViewModelAppender : IViewModelAppender
    {
        public bool Matches(RouteData routeData, string verb)
        {
            var controller = (string)routeData.Values["controller"];

            return HttpMethods.IsGet(verb)
                && controller.ToLowerInvariant() == "orders"
                && routeData.Values.ContainsKey("id");
        }

        public async Task Append(dynamic vm, RouteData routeData, IQueryCollection query)
        {
            var id = (string)routeData.Values["id"];

            var url = $"http://localhost:20295/api/orders/{id}";
            var client = new HttpClient();
            var response = await client.GetAsync(url).ConfigureAwait(false);

            dynamic order = await response.Content.AsExpandoAsync().ConfigureAwait(false);

            vm.OrderNumber = order.Number;
            vm.OrderItemsCount = order.ItemsCount;
        }
    }
}