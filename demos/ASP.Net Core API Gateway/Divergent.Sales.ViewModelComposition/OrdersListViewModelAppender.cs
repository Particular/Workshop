using Divergent.Sales.ViewModelComposition.Events;
using ITOps.ViewModelComposition;
using ITOps.ViewModelComposition.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System.Collections.Generic;
using System.Dynamic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Divergent.Sales.ViewModelComposition
{
    public class OrdersListViewModelAppender : IViewModelAppender
    {
        public bool Matches(RouteData routeData, string verb)
        {
            /*
             * matching is a bit weak in this sample, it's designed 
             * this way to satisfy both the Gateway and the Mvc sample
             */
            var controller = (string)routeData.Values["controller"];

            return HttpMethods.IsGet(verb)
                && controller.ToLowerInvariant() == "orders"
                && !routeData.Values.ContainsKey("id");
        }

        public async Task Append(dynamic vm, RouteData routeData, IQueryCollection query)
        {
            var pageIndex = (string)query["pageindex"] ?? "0";
            var pageSize = (string)query["pageSize"] ?? "10";

            var url = $"http://localhost:20295/api/orders?pageSize={pageSize}&pageIndex={pageIndex}";
            var client = new HttpClient();
            var response = await client.GetAsync(url).ConfigureAwait(false);

            dynamic[] orders = await response.Content.AsExpandoArrayAsync().ConfigureAwait(false);

            vm.OrdersViewModel = MapToDictionary(orders);

            await vm.RaiseEventAsync(new OrdersLoaded()
            {
                OrdersViewModel = vm.OrdersViewModel
            }).ConfigureAwait(false);
        }

        IDictionary<dynamic, dynamic> MapToDictionary(dynamic[] orders)
        {
            var ordersViewModel = new Dictionary<dynamic, dynamic>();

            foreach (dynamic order in orders)
            {
                dynamic vm = new ExpandoObject();
                vm.OrderNumber = order.Number;
                vm.OrderItemsCount = order.ItemsCount;

                ordersViewModel[order.Id] = vm;
            }

            return ordersViewModel;
        }
    }
}
