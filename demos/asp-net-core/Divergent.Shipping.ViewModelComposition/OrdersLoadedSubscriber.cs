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
        // It's written this way to satisfy both the composite gateway and website demos.
        public bool Matches(RouteData routeData, string httpMethod) =>
            HttpMethods.IsGet(httpMethod)
                && string.Equals((string)routeData.Values["controller"], "orders", StringComparison.OrdinalIgnoreCase)
                && !routeData.Values.ContainsKey("id");

        public void Subscribe(IPublishCompositionEvents publisher)
        {
            publisher.Subscribe<OrdersLoaded>(async (pageViewModel, ordersLoaded, routeData, query) =>
            {
                var orderNumbers = string.Join(",", ordersLoaded.OrderViewModelDictionary.Keys);

                // Hardcoded to simplify the demo. In a production app, a config object could be injected.
                var url = $"http://localhost:20296/api/shipments/orders?orderNumbers={orderNumbers}";
                var response = await new HttpClient().GetAsync(url);

                dynamic[] shipments = await response.Content.AsExpandoArrayAsync();

                foreach (dynamic shipment in shipments)
                {
                    ordersLoaded.OrderViewModelDictionary[shipment.OrderNumber].ShippingStatus = shipment.Status;
                    ordersLoaded.OrderViewModelDictionary[shipment.OrderNumber].ShippingCourier = shipment.Courier;
                }
            });
        }
    }
}
