using ITOps.Json;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using ServiceComposer.AspNetCore;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Divergent.Shipping.ViewModelComposition
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
            var url = $"http://localhost:20296/api/shipments/order/{id}";
            var response = await new HttpClient().GetAsync(url);

            dynamic shipment = await response.Content.AsExpandoAsync();

            vm.ShippingStatus = shipment.Status;
            vm.ShippingCourier = shipment.Courier;
        }
    }
}
