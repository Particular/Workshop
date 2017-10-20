using Divergent.Sales.ViewModelComposition.Events;
using ITOps.Json;
using ITOps.ViewModelComposition;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Divergent.Sales.ViewModelComposition
{
    public class OrdersListViewModelAppender : IViewModelAppender
    {
        string apiBaseAddress;

        public OrdersListViewModelAppender(IConfiguration config)
        {
            apiBaseAddress = config.GetSection("appenders:sales:apiBaseAddress").Value;
        }

        public bool Matches(RouteData routeData, string verb)
        {
            /*
             * matching is a bit weak in this sample, it's designed 
             * this way to satisfy both the composite gateway and website samples
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

            var url = $"{apiBaseAddress}/api/orders?pageSize={pageSize}&pageIndex={pageIndex}";
            var client = new HttpClient();
            var response = await client.GetAsync(url);

            dynamic[] orders = await response.Content.AsExpandoArrayAsync();

            var ordersViewModelDictionary = MapToDictionary(orders);

            await vm.RaiseEventAsync(new OrdersLoaded()
            {
                OrdersViewModel = ordersViewModelDictionary
            });

            vm.Orders = ordersViewModelDictionary.Values.ToArray();
        }

        IDictionary<dynamic, dynamic> MapToDictionary(dynamic[] orders)
        {
            var ordersViewModel = new Dictionary<dynamic, dynamic>();

            foreach (dynamic order in orders)
            {
                dynamic vm = new ExpandoObject();
                vm.OrderNumber = order.OrderNumber;
                vm.OrderItemsCount = order.ItemsCount;

                ordersViewModel[order.OrderNumber] = vm;
            }

            return ordersViewModel;
        }
    }
}
