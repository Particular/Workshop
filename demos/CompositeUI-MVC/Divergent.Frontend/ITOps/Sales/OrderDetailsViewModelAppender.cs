using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Routing;

namespace Divergent.Frontend.ITOps.Sales
{
    public class OrderDetailsViewModelAppender : IViewModelAppender
    {
        public Task Append(RouteData routeData, dynamic viewModel)
        {
            return Task.Run(async () =>
            {
                var id = (string)routeData.Values["id"];

                var url = $"http://localhost:20195/api/orders/{id}";
                var client = new HttpClient();
                var response = await client.GetAsync(url);

                viewModel.Order = await response.Content.AsExpandoAsync();
            });
        }

        public bool Matches(RouteData routeData)
        {
            var controller = (string)routeData.Values["controller"];
            var action = (string)routeData.Values["action"];

            return controller == "Orders" && action == "Details";
        }
    }
}