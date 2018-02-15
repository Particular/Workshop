using Divergent.Sales.ViewModelComposition.Events;
using ITOps.Json;
using ITOps.ViewModelComposition;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Divergent.Sales.ViewModelComposition
{
    public class OrdersListViewModelAppender : IViewModelAppender
    {
        // Matching is a bit weak in this demo.
        // It's written this way to satisfy both the composite gateway and website demos.
        public bool Matches(RouteData routeData, string httpMethod) =>
            HttpMethods.IsGet(httpMethod)
                && string.Equals((string)routeData.Values["controller"], "orders", StringComparison.OrdinalIgnoreCase)
                && !routeData.Values.ContainsKey("id");

        public async Task Append(dynamic viewModel, RouteData routeData, IQueryCollection query)
        {
            var pageIndex = (string)query["pageindex"] ?? "0";
            var pageSize = (string)query["pageSize"] ?? "10";

            // Hardcoded to simplify the demo. In a production app, a config object could be injected.
            var url = $"http://localhost:20295/api/orders?pageSize={pageSize}&pageIndex={pageIndex}";
            var response = await new HttpClient().GetAsync(url);

            dynamic[] orders = await response.Content.AsExpandoArrayAsync();

            var orderViewModelDictionary = MapToViewModelDictionary(orders);

            await viewModel.RaiseEventAsync(new OrdersLoaded { OrderViewModelDictionary = orderViewModelDictionary });

            viewModel.Orders = orderViewModelDictionary.Values;
        }

        IDictionary<dynamic, dynamic> MapToViewModelDictionary(dynamic[] orders)
        {
            var dictionary = new Dictionary<dynamic, dynamic>();

            foreach (var order in orders)
            {
                dynamic viewModel = new ExpandoObject();
                viewModel.OrderNumber = order.OrderNumber;
                viewModel.OrderItemsCount = order.ItemsCount;

                dictionary[order.OrderNumber] = viewModel;
            }

            return dictionary;
        }
    }
}
