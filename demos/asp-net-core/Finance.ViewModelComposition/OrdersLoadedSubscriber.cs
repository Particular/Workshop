using Divergent.Sales.ViewModelComposition.Events;
using ITOps.ViewModelComposition;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Text;

namespace Finance.ViewModelComposition
{
    class OrdersLoadedSubscriber : ISubscribeToCompositionEvents
    {
        public bool Matches(RouteData routeData, string httpMethod) =>
HttpMethods.IsGet(httpMethod)
                && string.Equals((string)routeData.Values["controller"], "orders", StringComparison.OrdinalIgnoreCase)
                && !routeData.Values.ContainsKey("id");

        public void Subscribe(IPublishCompositionEvents publisher)
        {
            publisher.Subscribe<OrdersLoaded>(async (pageViewModel, ordersLoaded, routeData, query) =>
            {
                var orderNumbers = string.Join(",", ordersLoaded.OrderViewModelDictionary.Keys);

// use the order numbers to find the prices

                foreach (dynamic order in ordersLoaded.OrderViewModelDictionary)
                {
                    order.Value.Price = 100;
                }
            });
        }
    }
}
