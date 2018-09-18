using ITOps.ViewModelComposition;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System;
using System.Threading.Tasks;

namespace Finance.ViewModelComposition
{
    public class OrderDetailsViewModelAppender : IViewModelAppender
    {
        public Task Append(dynamic viewModel, RouteData routeData, IQueryCollection query)
        {
            // go to back the end get the prices for each order...
            viewModel.Price = 100;
            return Task.CompletedTask;
        }

        public bool Matches(RouteData routeData, string httpMethod)=>
            HttpMethods.IsGet(httpMethod)
                && string.Equals((string)routeData.Values["controller"], "orders", StringComparison.OrdinalIgnoreCase)
                && routeData.Values.ContainsKey("id");
    }
}
