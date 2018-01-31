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
        public bool Matches(RouteData routeData, string httpMethod) =>
            HttpMethods.IsGet(httpMethod)
                && string.Equals((string)routeData.Values["controller"], "orders", StringComparison.OrdinalIgnoreCase)
                && !routeData.Values.ContainsKey("id");

        public async Task Append(dynamic viewModel, RouteData routeData, IQueryCollection query)
        {
            // Hardcoded for simplicity. In a production app, a config object could be injected.
            var url = $"http://localhost:20185/api/orders";
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
                viewModel.OrderId = order.Id;
                viewModel.OrderNumber = order.Id;

                dictionary[order.Id] = viewModel;
            }

            return dictionary;
        }
    }
}