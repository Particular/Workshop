using Divergent.Sales.ViewModelComposition.Events;
using ITOps.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using ServiceComposer.AspNetCore;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Divergent.Sales.ViewModelComposition
{
    public class OrdersListViewModelAppender : ICompositionRequestsHandler
    {
        IDictionary<dynamic, dynamic> MapToViewModelDictionary(dynamic[] orders)
        {
            var dictionary = new Dictionary<dynamic, dynamic>();

            foreach (var order in orders)
            {
                dynamic viewModel = new ExpandoObject();
                viewModel.OrderNumber = order.OrderNumber;
                viewModel.OrderItemsCount = order.Items.Count;

                dictionary[order.OrderNumber] = viewModel;
            }

            return dictionary;
        }

        [HttpGet("/orders")]
        public async Task Handle(HttpRequest request)
        {
            var pageIndex = (string)request.Query["pageindex"] ?? "0";
            var pageSize = (string)request.Query["pageSize"] ?? "10";

            // Hardcoded to simplify the demo. In a production app, a config object could be injected.
            var url = $"http://localhost:20295/api/orders?pageSize={pageSize}&pageIndex={pageIndex}";
            var response = await new HttpClient().GetAsync(url);

            dynamic[] orders = await response.Content.AsExpandoArrayAsync();

            var orderViewModelDictionary = MapToViewModelDictionary(orders);

            var compositionContext = request.GetCompositionContext();
            await compositionContext.RaiseEvent(new OrdersLoaded { OrderViewModelDictionary = orderViewModelDictionary });

            // Using dynamic to simplify the demo. The ViewModel can be strongly typed.
            var vm = request.GetComposedResponseModel();
            vm.Orders = orderViewModelDictionary.Values;
        }
    }
}
