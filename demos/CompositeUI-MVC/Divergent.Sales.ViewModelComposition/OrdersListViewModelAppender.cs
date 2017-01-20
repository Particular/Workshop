using Divergent.ITOps.ViewModelComposition;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Routing;
using System.Linq;
using System.Dynamic;
using Divergent.Sales.ViewModelComposition.Events;

namespace Divergent.Sales.ViewModelComposition
{
    public class OrdersListViewModelAppender : IViewModelAppender
    {
        public Task Append(RouteData routeData, dynamic viewModel)
        {
            return Task.Run(async () =>
            {
                var pageIndex = (string)routeData.Values["pageindex"] ?? "0";
                var pageSize = (string)routeData.Values["pageSize"] ?? "10";

                var url = $"http://localhost:20195/api/orders?pageSize={pageSize}&pageIndex={pageIndex}";
                var client = new HttpClient();
                var response = await client.GetAsync(url);

                dynamic[] orders = await response.Content.AsExpandoArrayAsync();

                var orderViewModels = orders.ToDictionary(o => o.Id, o =>
                {
                    dynamic vm = new ExpandoObject();
                    vm.OrderNumber = o.Number;
                    vm.OrderItemsCount = o.ItemsCount;

                    return vm;
                });

                viewModel.Orders = orderViewModels.Values;

                viewModel.RaiseEvent(new OrdersLoaded() { Orders = orderViewModels });
            });
        }

        public bool Matches(RouteData routeData)
        {
            var controller = (string)routeData.Values["controller"];
            var action = (string)routeData.Values["action"];

            return controller == "Orders" && action == "Index";
        }
    }
}