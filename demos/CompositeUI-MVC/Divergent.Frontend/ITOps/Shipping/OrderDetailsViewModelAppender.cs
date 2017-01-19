using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Routing;

namespace Divergent.Frontend.ITOps.Shipping
{
    public class OrderDetailsViewModelAppender : IViewModelAppender
    {
        public Task Append(RouteData routeData, dynamic viewModel)
        {
            return Task.Run(async () =>
            {
                var id = (string)routeData.Values["id"];

                var url = $"http://localhost:20196/api/shippinginfo/order/{id}";
                var client = new HttpClient();
                var response = await client.GetAsync(url);

                dynamic shipping = await response.Content.AsExpandoAsync();

                viewModel.ShippingStatus = shipping.Status;
                viewModel.ShippingCourier = shipping.Courier;
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