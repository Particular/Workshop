using Divergent.ITOps.ViewModelComposition;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Routing;
using System;
using Divergent.Sales.ViewModelComposition.Events;
using System.Dynamic;
using System.Linq;

namespace Divergent.Shipping.ViewModelComposition
{
    public class OrdersLoadedSubscriber : ISubscribeToCompositionEvents
    {
        public bool Matches(ITOps.ViewModelComposition.RequestContext request)
        {
            var controller = (string)request.RouteData.Values["controller"];
            var action = (string)request.RouteData.Values["action"];

            return controller == "Orders" && action == "Index";
        }

        public void Subscribe(ISubscriptionStorage subscriptionStorage)
        {
            subscriptionStorage.Subscribe<OrdersLoaded>(async (pageViewModel, @event, request) =>
            {
                var ids = String.Join(",", @event.OrdersViewModel.Keys);
                var url = $"http://localhost:20196/api/shippinginfo/orders?ids={ids}";
                var client = new HttpClient();

                var response = await client.GetAsync(url);

                dynamic[] shippingInfos = await response.Content.AsExpandoArrayAsync();

                foreach (dynamic item in shippingInfos)
                {
                    @event.OrdersViewModel[item.OrderId].ShippingStatus = item.Status;
                    @event.OrdersViewModel[item.OrderId].ShippingCourier = item.Courier;
                }
            });
        }
    }
}