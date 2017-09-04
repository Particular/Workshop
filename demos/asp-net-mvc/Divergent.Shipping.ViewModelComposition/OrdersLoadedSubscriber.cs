using Divergent.ITOps.ViewModelComposition;
using Divergent.Sales.ViewModelComposition.Events;
using System.Net.Http;

namespace Divergent.Shipping.ViewModelComposition
{
    public class OrdersLoadedSubscriber : ISubscribeToCompositionEvents
    {
        public bool Matches(RequestContext request)
        {
            var controller = (string)request.RouteData.Values["controller"];
            var action = (string)request.RouteData.Values["action"];

            return controller == "Orders" && action == "Index";
        }

        public void Subscribe(ISubscriptionStorage subscriptionStorage)
        {
            subscriptionStorage.Subscribe<OrdersLoaded>(async (pageViewModel, @event, request) =>
            {
                var ids = string.Join(",", @event.OrdersViewModel.Keys);
                var url = $"http://localhost:20196/api/shippinginfo/orders?ids={ids}";
                var client = new HttpClient();

                var response = await client.GetAsync(url).ConfigureAwait(false);

                dynamic[] shippingInfos = await response.Content.AsExpandoArrayAsync().ConfigureAwait(false);

                foreach (var item in shippingInfos)
                {
                    @event.OrdersViewModel[item.OrderId].ShippingStatus = item.Status;
                    @event.OrdersViewModel[item.OrderId].ShippingCourier = item.Courier;
                }
            });
        }
    }
}