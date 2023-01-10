using ITOps.Json;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using ServiceComposer.AspNetCore;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Divergent.Sales.ViewModelComposition
{
    public class OrderDetailsViewModelAppender : ICompositionRequestsHandler
    {
        [HttpGet("/orders/details/{id}")]
        public async Task Handle(HttpRequest request)
        {
            var id = (string)request.HttpContext.GetRouteData().Values["id"];

            // Using dynamic to simplify the demo. The ViewModel can be strongly typed.
            var vm = request.GetComposedResponseModel();

            // Hardcoded to simplify the demo. In a production app, a config object could be injected.
            var url = $"http://localhost:20295/api/orders/{id}";
            var response = await new HttpClient().GetAsync(url);

            dynamic order = await response.Content.AsExpandoAsync();

            vm.OrderNumber = order.OrderNumber;
            vm.OrderItemsCount = order.ItemsCount;
        }
    }
}
