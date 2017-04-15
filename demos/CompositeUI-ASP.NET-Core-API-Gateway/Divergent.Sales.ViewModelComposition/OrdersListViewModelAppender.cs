using System.Net.Http;
using System.Threading.Tasks;
using System.Dynamic;
using System.Collections.Generic;
using ITOps.ViewModelComposition;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Http;
using Divergent.Sales.ViewModelComposition.Events;

namespace Divergent.Sales.ViewModelComposition
{
    public class OrdersListViewModelAppender : IViewModelAppender
    {
        public bool Matches(RouteData routeData, string verb)
        {
            var controller = (string)routeData.Values["controller"];
            //var action = (string)request.RouteData.Values["action"];

            return HttpMethods.IsGet(verb)
                && controller.ToLowerInvariant() == "orders"
                && !routeData.Values.ContainsKey("id");
        }

        public async Task Append(dynamic vm, RouteData routeData, IQueryCollection query)
        {
            var pageIndex = (string)query["pageindex"] ?? "0";
            var pageSize = (string)query["pageSize"] ?? "10";

            //var url = $"http://localhost:20195/api/orders?pageSize={pageSize}&pageIndex={pageIndex}";
            //var client = new HttpClient();
            //var response = await client.GetAsync(url).ConfigureAwait(false);

            //dynamic[] orders = await response.Content.AsExpandoArrayAsync().ConfigureAwait(false);

            vm.OrdersViewModel = MapToDictionary(/*orders*/);

            await vm.RaiseEventAsync(new OrdersLoaded()
            {
                OrdersViewModel = vm.OrdersViewModel
            }).ConfigureAwait(false);
        }

        //IDictionary<dynamic, dynamic> MapToDictionary(dynamic[] orders)
        //{
        //    var ordersViewModel = new Dictionary<dynamic, dynamic>();

        //    foreach (dynamic order in orders)
        //    {
        //        dynamic vm = new ExpandoObject();
        //        vm.OrderNumber = order.Number;
        //        vm.OrderItemsCount = order.ItemsCount;

        //        ordersViewModel[order.Id] = vm;
        //    }

        //    return ordersViewModel;
        //}

        IDictionary<dynamic, dynamic> MapToDictionary()
        {
            var ordersViewModel = new Dictionary<dynamic, dynamic>();

            for (int i = 0; i < 5; i++)
            {
                dynamic vm = new ExpandoObject();
                vm.OrderNumber = i;
                vm.OrderItemsCount = i * 5;

                ordersViewModel[i] = vm;
            }

            return ordersViewModel;
        }
    }
}
