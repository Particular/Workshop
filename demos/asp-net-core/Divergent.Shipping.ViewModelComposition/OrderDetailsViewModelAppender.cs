using ITOps.Json;
using ITOps.ViewModelComposition;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Divergent.Shipping.ViewModelComposition
{
    public class OrderDetailsViewModelAppender : IViewModelAppender
    {
        // Matching is a bit weak in this demo.
        // It's written this way to satisfy both the composite gateway and website demos.
        public bool Matches(RouteData routeData, string httpMethod) =>
            HttpMethods.IsGet(httpMethod)
                && string.Equals((string)routeData.Values["controller"], "orders", StringComparison.OrdinalIgnoreCase)
                && routeData.Values.ContainsKey("id");

        public async Task Append(dynamic viewModel, RouteData routeData, IQueryCollection query)
        {
            var id = (string)routeData.Values["id"];

            // Hardcoded to simplify the demo. In a production app, a config object could be injected.
            var url = $"http://localhost:20296/api/shipments/order/{id}";
            var response = await new HttpClient().GetAsync(url);

            dynamic shipment = await response.Content.AsExpandoAsync();

            viewModel.ShippingStatus = shipment.Status;
            viewModel.ShippingCourier = shipment.Courier;
        }
    }
}
