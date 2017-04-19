using ITOps.ViewModelComposition;
using ITOps.ViewModelComposition.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System.Net.Http;
using System.Threading.Tasks;

namespace Divergent.Shipping.ViewModelComposition
{
    public class OrderDetailsViewModelAppender : IViewModelAppender
    {
        public bool Matches(RouteData routeData, string verb)
        {
            /*
             * matching is a bit weak in this sample, it's designed 
             * this way to satisfy both the Gateway and the Mvc sample
             */
            var controller = (string)routeData.Values["controller"];

            return HttpMethods.IsGet(verb)
                && controller.ToLowerInvariant() == "orders"
                && routeData.Values.ContainsKey("id");
        }

        public async Task Append(dynamic vm, RouteData routeData, IQueryCollection query)
        {
            var id = (string)routeData.Values["id"];

            var url = $"http://localhost:20296/api/shippinginfo/order/{id}";
            var client = new HttpClient();
            var response = await client.GetAsync(url).ConfigureAwait(false);

            dynamic shipping = await response.Content.AsExpandoAsync().ConfigureAwait(false);

            vm.ShippingStatus = shipping.Status;
            vm.ShippingCourier = shipping.Courier;
        }
    }
}