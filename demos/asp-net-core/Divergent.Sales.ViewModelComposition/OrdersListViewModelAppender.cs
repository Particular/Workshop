using Divergent.Sales.ViewModelComposition.Events;
using ITOps.Json;
using ITOps.ViewModelComposition;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Divergent.Sales.ViewModelComposition
{
    public class OrdersListViewModelAppender : IViewModelAppender
    {
        // Matching is a bit weak in this demo.
        // It's written this way to satisfy both the composite gateway and website samples.
        public bool Matches(RouteData routeData, string verb) =>
            HttpMethods.IsGet(verb)
                && string.Equals((string)routeData.Values["controller"], "orders", StringComparison.OrdinalIgnoreCase)
                && !routeData.Values.ContainsKey("id");

        public async Task Append(dynamic vm, RouteData routeData, IQueryCollection query)
        {
            var pageIndex = (string)query["pageindex"] ?? "0";
            var pageSize = (string)query["pageSize"] ?? "10";

            // Hardcoded to simplify the demo. In a production app, a config object could be injected.
            var url = $"http://localhost:20295/api/orders?pageSize={pageSize}&pageIndex={pageIndex}";
            var response = await new HttpClient().GetAsync(url);

            dynamic[] orders = await response.Content.AsExpandoArrayAsync();

            var ordersViewModel = MapToViewModel(orders);

            await vm.RaiseEventAsync(new OrdersLoaded { OrdersViewModel = ordersViewModel });

            vm.Orders = ordersViewModel.Values.ToArray();
        }

        IDictionary<dynamic, dynamic> MapToViewModel(dynamic[] orders)
        {
            var ordersViewModel = new Dictionary<dynamic, dynamic>();

            foreach (var order in orders)
            {
                dynamic orderViewModel = new ExpandoObject();
                orderViewModel.OrderNumber = order.OrderNumber;
                orderViewModel.OrderItemsCount = order.ItemsCount;

                ordersViewModel[order.OrderNumber] = orderViewModel;
            }

            return ordersViewModel;
        }
    }
}
