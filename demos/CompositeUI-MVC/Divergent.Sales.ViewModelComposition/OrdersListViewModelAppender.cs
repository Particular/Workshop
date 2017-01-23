using Divergent.ITOps.ViewModelComposition;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Routing;
using System.Linq;
using System.Dynamic;
using Divergent.Sales.ViewModelComposition.Events;
using System.Collections.Generic;

namespace Divergent.Sales.ViewModelComposition
{
    public class OrdersListViewModelAppender : IViewModelAppender
    {
        public bool Matches(ITOps.ViewModelComposition.RequestContext request)
        {
            var controller = (string)request.RouteData.Values["controller"];
            var action = (string)request.RouteData.Values["action"];

            return controller == "Orders" && action == "Index";
        }

        public Task Append(ITOps.ViewModelComposition.RequestContext request, dynamic viewModel)
        {
            return Task.Run(async () =>
            {
                var pageIndex = (string)request.QueryString["pageindex"] ?? "0";
                var pageSize = (string)request.QueryString["pageSize"] ?? "10";

                var url = $"http://localhost:20195/api/orders?pageSize={pageSize}&pageIndex={pageIndex}";
                var client = new HttpClient();
                var response = await client.GetAsync(url);

                dynamic[] orders = await response.Content.AsExpandoArrayAsync();

                //var ordersDictionary = orders.ToDictionary(o => o.Id, o =>
                //{
                //    dynamic vm = new ExpandoObject();
                //    vm.OrderNumber = o.Number;
                //    vm.OrderItemsCount = o.ItemsCount;

                //    return vm;
                //});

                var ordersViewModel = new Dictionary<dynamic, dynamic>();

                foreach (dynamic order in orders)
                {
                    ordersViewModel[order.Id] = new ExpandoObject();
                    ordersViewModel[order.Id].OrderNumber = order.Number;
                    ordersViewModel[order.Id].OrderItemsCount = order.ItemsCount;
                }

                viewModel.RaiseEvent(new OrdersLoaded()
                {
                    OrdersViewModel = ordersViewModel
                });

                viewModel.OrdersCount = orders.Length;
                viewModel.OrdersViewModel = ordersViewModel;
            });
        }
    }
}