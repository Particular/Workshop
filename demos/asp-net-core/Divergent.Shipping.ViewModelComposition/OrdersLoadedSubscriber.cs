using Divergent.Sales.ViewModelComposition.Events;
using ITOps.Json;
using ServiceComposer.AspNetCore;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System;
using System.Net.Http;
using Microsoft.AspNetCore.Mvc;

namespace Divergent.Shipping.ViewModelComposition
{
    public class OrdersLoadedSubscriber : ICompositionEventsSubscriber
    {
        [HttpGet("/orders")]
        public void Subscribe(ICompositionEventsPublisher publisher)
        {
            publisher.Subscribe<OrdersLoaded>(async (@event, httpRequest) => 
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
