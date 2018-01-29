using Divergent.Sales.ViewModelComposition.Events;
using ITOps.Json;
using ITOps.ViewModelComposition;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System;
using System.Net.Http;

namespace Divergent.Customers.ViewModelComposition
{
    public class OrdersLoadedSubscriber : ISubscribeToCompositionEvents
    {
        // Very simple matching for the purpose of the exercise.
        public bool Matches(RouteData routeData, string httpMethod) =>
            HttpMethods.IsGet(httpMethod)
                && string.Equals((string)routeData.Values["controller"], "orders", StringComparison.OrdinalIgnoreCase)
                && !routeData.Values.ContainsKey("id");

        public void Subscribe(IPublishCompositionEvents publisher)
        {
            publisher.Subscribe<OrdersLoaded>(async (pageViewModel, ordersLoaded, routeData, query) =>
            {
                var orderIds = string.Join(",", ordersLoaded.OrderViewModelDictionary.Keys);

                // Hardcoded to simplify the exercise. In a production app, a config object could be injected.
                var url = $"http://localhost:20186/api/customers/byorders?orderIds={orderIds}";
                var response = await new HttpClient().GetAsync(url);

                dynamic[] customers = await response.Content.AsExpandoArrayAsync();

                foreach (dynamic customer in customers)
                {
                    ordersLoaded.OrderViewModelDictionary[customer.OrderId].OrderCustomerName = customer.CustomerName;
                }
            });
        }
    }
}