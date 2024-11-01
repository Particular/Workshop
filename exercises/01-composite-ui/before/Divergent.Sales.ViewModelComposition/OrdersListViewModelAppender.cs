using System.Collections.Generic;
using System.Dynamic;
using System.Net.Http;
using System.Threading.Tasks;
using Divergent.Sales.ViewModelComposition.Events;
using ITOps.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ServiceComposer.AspNetCore;

namespace Divergent.Sales.ViewModelComposition;

public class OrdersListViewModelAppender : ICompositionRequestsHandler
{
    [HttpGet("/orders")]
    public async Task Handle(HttpRequest request)
    {
        // Hardcoded for simplicity. In a production app, a config object could be injected.
        var url = $"http://localhost:20185/api/orders";
        var response = await new HttpClient().GetAsync(url);

        dynamic[] orders = await response.Content.AsExpandoArrayAsync();

        var orderViewModelDictionary = MapToViewModelDictionary(orders);

        var compositionContext = request.GetCompositionContext();
        await compositionContext.RaiseEvent(new OrdersLoaded { OrderViewModelDictionary = orderViewModelDictionary });

        var viewModel = request.GetComposedResponseModel();
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