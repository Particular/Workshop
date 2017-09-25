using Divergent.ITOps.ViewModelComposition;
using Divergent.Sales.ViewModelComposition.Events;
using System.Collections.Generic;
using System.Dynamic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Divergent.Sales.ViewModelComposition
{
    public class OrdersListViewModelAppender : IViewModelAppender
    {
        public bool Matches(RequestContext request)
        {
            var controller = (string)request.RouteData.Values["controller"];
            var action = (string)request.RouteData.Values["action"];

            return controller == "Orders" && action == "Index";
        }

        public async Task Append(RequestContext request, dynamic viewModel)
        {
            var pageIndex = (string)request.QueryString["pageindex"] ?? "0";
            var pageSize = (string)request.QueryString["pageSize"] ?? "10";

            var url = $"http://localhost:20195/api/orders?pageSize={pageSize}&pageIndex={pageIndex}";
            var client = new HttpClient();
            var response = await client.GetAsync(url).ConfigureAwait(false);

            dynamic[] orders = await response.Content.AsExpandoArrayAsync().ConfigureAwait(false);

            viewModel.OrdersViewModel = MapToDictionary(orders);

            await viewModel.RaiseEventAsync(new OrdersLoaded
            {
                OrdersViewModel = viewModel.OrdersViewModel
            }).ConfigureAwait(false);
        }

        IDictionary<dynamic, dynamic> MapToDictionary(dynamic[] orders)
        {
            var ordersViewModel = new Dictionary<dynamic, dynamic>();

            foreach (var order in orders)
            {
                ordersViewModel[order.Id] = new ExpandoObject();
                ordersViewModel[order.Id].OrderNumber = order.Number;
                ordersViewModel[order.Id].OrderItemsCount = order.ItemsCount;
            }

            return ordersViewModel;
        }
    }
}
