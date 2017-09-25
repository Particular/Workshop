using Divergent.ITOps.ViewModelComposition;
using System.Net.Http;
using System.Threading.Tasks;

namespace Divergent.Sales.ViewModelComposition
{
    public class OrderDetailsViewModelAppender : IViewModelAppender
    {
        public bool Matches(RequestContext request)
        {
            var controller = (string)request.RouteData.Values["controller"];
            var action = (string)request.RouteData.Values["action"];

            return controller == "Orders" && action == "Details";
        }

        public async Task Append(RequestContext request, dynamic viewModel)
        {
            var id = (string)request.RouteData.Values["id"];

            var url = $"http://localhost:20195/api/orders/{id}";
            var client = new HttpClient();
            var response = await client.GetAsync(url).ConfigureAwait(false);

            dynamic order = await response.Content.AsExpandoAsync().ConfigureAwait(false);

            viewModel.OrderNumber = order.Number;
            viewModel.OrderItemsCount = order.ItemsCount;
        }
    }
}