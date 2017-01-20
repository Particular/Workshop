using Divergent.ITOps.ViewModelComposition;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Routing;

namespace Divergent.Sales.ViewModelComposition
{
    public class OrderDetailsViewModelAppender : IViewModelAppender
    {
        public Task Append(RouteData routeData, dynamic viewModel)
        {
            return Task.Run(async () =>
            {
                var id = (string)routeData.Values["id"];

                var url = $"http://localhost:20195/api/orders/{id}";
                var client = new HttpClient();
                var response = await client.GetAsync(url);

                dynamic order= await response.Content.AsExpandoAsync();

                viewModel.OrderNumber = order.Number;
                viewModel.OrderItemsCount = order.ItemsCount;
            });
        }

        public bool Matches(RouteData routeData)
        {
            var controller = (string)routeData.Values["controller"];
            var action = (string)routeData.Values["action"];

            return controller == "Orders" && action == "Details";
        }
    }
}