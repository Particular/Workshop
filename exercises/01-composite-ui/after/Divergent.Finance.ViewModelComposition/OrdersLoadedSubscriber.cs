using System.Net.Http;
using Divergent.Sales.ViewModelComposition.Events;
using ITOps.Json;
using Microsoft.AspNetCore.Mvc;
using ServiceComposer.AspNetCore;

namespace Divergent.Finance.ViewModelComposition;

public class OrdersLoadedSubscriber : ICompositionEventsSubscriber
{
    [HttpGet("/orders")]
    public void Subscribe(ICompositionEventsPublisher publisher)
    {
        publisher.Subscribe<OrdersLoaded>(async (ordersLoaded, httpRequest) =>
        {
            var orderIds = string.Join(",", ordersLoaded.OrderViewModelDictionary.Keys);

            // Hardcoded to simplify the exercise. In a production app, a config object could be injected.
            var url = $"http://localhost:20187/api/prices/orders/total?orderIds={orderIds}";
            var response = await new HttpClient().GetAsync(url);

            dynamic[] prices = await response.Content.AsExpandoArrayAsync();

            foreach (dynamic price in prices)
            {
                ordersLoaded.OrderViewModelDictionary[price.OrderId].OrderTotalPrice = price.Amount;
            }
        });
    }
}