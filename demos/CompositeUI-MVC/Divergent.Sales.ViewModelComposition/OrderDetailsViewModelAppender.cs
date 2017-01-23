using Divergent.ITOps.ViewModelComposition;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Routing;

namespace Divergent.Sales.ViewModelComposition
{
    public class OrderDetailsViewModelAppender : IViewModelAppender
    {
        public bool Matches(RequestInfo request)
        {
            var controller = (string)request.RouteData.Values["controller"];
            var action = (string)request.RouteData.Values["action"];

            return controller == "Orders" && action == "Details";
        }

        public Task Append(RequestInfo request, dynamic viewModel)
        {
            return Task.Run(async () =>
            {
                var id = (string)request.RouteData.Values["id"];

                var url = $"http://localhost:20195/api/orders/{id}";
                var client = new HttpClient();
                var response = await client.GetAsync(url);

                dynamic order= await response.Content.AsExpandoAsync();

                viewModel.OrderNumber = order.Number;
                viewModel.OrderItemsCount = order.ItemsCount;
            });
        }
    }
}