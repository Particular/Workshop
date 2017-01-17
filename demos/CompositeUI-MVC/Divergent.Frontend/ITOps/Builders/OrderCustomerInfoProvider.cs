using System.Net.Http;
using System.Threading.Tasks;

namespace Divergent.Frontend.ITOps.Builders
{
    public class OrderCustomerInfoProvider
    {
        public async Task<dynamic> BuildCustomerViewModel(dynamic customerId)
        {
            var url = "http://localhost:20186/api/customers?id=" + customerId;
            var client = new HttpClient();
            HttpResponseMessage response = await client.GetAsync(url);
            dynamic vm = await response.Content.AsExpandoAsync();

            return vm;
        }
    }
}