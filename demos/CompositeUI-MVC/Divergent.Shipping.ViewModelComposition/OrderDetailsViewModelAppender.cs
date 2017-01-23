using Divergent.ITOps.ViewModelComposition;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Routing;

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

        public Task Append(ITOps.ViewModelComposition.RequestContext request, dynamic viewModel)
        {
            return Task.Run(async () =>
            {
                var id = (string)request.RouteData.Values["id"];

                var url = $"http://localhost:20196/api/shippinginfo/order/{id}";
                var client = new HttpClient();
                var response = await client.GetAsync(url);

                dynamic shipping = await response.Content.AsExpandoAsync();

                viewModel.ShippingStatus = shipping.Status;
                viewModel.ShippingCourier = shipping.Courier;
            });
        }
    }
}