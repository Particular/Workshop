using Divergent.Sales.ViewModelComposition.Events;
using ITOps.Json;
using ITOps.ViewModelComposition;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System;
using System.Net.Http;

namespace Divergent.Shipping.ViewModelComposition
{
    public class OrdersLoadedSubscriber : ISubscribeToCompositionEvents
    {
        // Matching is a bit weak in this demo.
        // It's written this way to satisfy both the composite gateway and website samples.
        public bool Matches(RouteData routeData, string verb) =>
            HttpMethods.IsGet(verb)
                && string.Equals((string)routeData.Values["controller"], "orders", StringComparison.OrdinalIgnoreCase)
                && !routeData.Values.ContainsKey("id");

        public void Subscribe(ISubscriptionStorage subscriptionStorage, RouteData rd, IQueryCollection queryString)
        {
            subscriptionStorage.Subscribe<OrdersLoaded>(async (pageViewModel, @event, routeData, query) =>
            {
                var orderNumbers = string.Join(",", @event.OrderViewModelDictionary.Keys);

                // Hardcoded to simplify the demo. In a production app, a config object could be injected.
                var url = $"http://localhost:20296/api/shipments/orders?orderNumbers={orderNumbers}";
                var response = await new HttpClient().GetAsync(url);

                dynamic[] shipments = await response.Content.AsExpandoArrayAsync();

                foreach (dynamic shipment in shipments)
                {
                    @event.OrderViewModelDictionary[shipment.OrderNumber].ShippingStatus = shipment.Status;
                    @event.OrderViewModelDictionary[shipment.OrderNumber].ShippingCourier = shipment.Courier;
                }
            });
        }
    }
}
