using System.Net.Http;
using System;
using Divergent.Sales.ViewModelComposition.Events;
using ITOps.ViewModelComposition;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System.Threading.Tasks;

namespace Divergent.Shipping.ViewModelComposition
{
    public class OrdersLoadedSubscriber : ISubscribeToCompositionEvents
    {
        public bool Matches(RouteData routeData, string httpVerb)
        {
            var controller = (string)routeData.Values["controller"];

            return HttpMethods.IsGet(httpVerb)
                && controller.ToLowerInvariant() == "orders"
                && !routeData.Values.ContainsKey("id");
        }

        public void Subscribe(ISubscriptionStorage subscriptionStorage, RouteData rd, IQueryCollection queryString)
        {
            subscriptionStorage.Subscribe<OrdersLoaded>((pageViewModel, @event, routeData, query) =>
            {
                var ids = String.Join(",", @event.OrdersViewModel.Keys);

                //var url = $"http://localhost:20196/api/shippinginfo/orders?ids={ids}";
                //var client = new HttpClient();

                //var response = await client.GetAsync(url).ConfigureAwait(false);

                //dynamic[] shippingInfos = await response.Content.AsExpandoArrayAsync().ConfigureAwait(false);

                //foreach (dynamic item in shippingInfos)
                //{
                //    @event.OrdersViewModel[item.OrderId].ShippingStatus = item.Status;
                //    @event.OrdersViewModel[item.OrderId].ShippingCourier = item.Courier;
                //}

                foreach (var item in @event.OrdersViewModel)
                {
                    item.Value.ShippingStatus = $"Status of {item.Key}";
                    item.Value.ShippingCourier = $"Courier of {item.Key}";
                }

                return Task.CompletedTask;
            });
        }
    }
}