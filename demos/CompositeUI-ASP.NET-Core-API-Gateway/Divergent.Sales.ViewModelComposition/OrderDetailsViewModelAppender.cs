using ITOps.ViewModelComposition;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

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

        public Task Append(dynamic vm, RouteData routeData, IQueryCollection query)
        {
            var id = (string)routeData.Values["id"];

            //var url = $"http://localhost:20195/api/orders/{id}";
            //var client = new HttpClient();
            //var response = await client.GetAsync(url).ConfigureAwait(false);

            //dynamic order = await response.Content.AsExpandoAsync().ConfigureAwait(false);

            vm.OrderNumber = id;
            vm.OrderItemsCount = 120;

            return Task.CompletedTask;
        }
    }
}