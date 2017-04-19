using Divergent.ITOps.ViewModelComposition;
using System.Net.Http;
using System.Threading.Tasks;

namespace Divergent.Shipping.ViewModelComposition
{
    public class OrderDetailsViewModelAppender : IViewModelAppender
    {
        public bool Matches(ITOps.ViewModelComposition.RequestContext request)
        {
            var controller = (string)request.RouteData.Values["controller"];
            var action = (string)request.RouteData.Values["action"];

            return controller == "Orders" && action == "Details";
        }

        public async Task Append(ITOps.ViewModelComposition.RequestContext request, dynamic viewModel)
        {
            var id = (string)request.RouteData.Values["id"];

            var url = $"http://localhost:20196/api/shippinginfo/order/{id}";
            var client = new HttpClient();
            var response = await client.GetAsync(url).ConfigureAwait(false);

            dynamic shipping = await response.Content.AsExpandoAsync().ConfigureAwait(false);

            viewModel.ShippingStatus = shipping.Status;
            viewModel.ShippingCourier = shipping.Courier;
        }
    }
}