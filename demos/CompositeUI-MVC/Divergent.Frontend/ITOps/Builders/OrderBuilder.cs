using System.Net.Http;
using System.Threading.Tasks;
using System.Dynamic;

namespace Divergent.Frontend.ITOps.Builders
{
    public class OrderBuilder
    {
        private OrderCustomerInfoProvider orderCustomerInfoProvider;

        public OrderBuilder(OrderCustomerInfoProvider orderCustomerInfoProvider)
        {
            this.orderCustomerInfoProvider = orderCustomerInfoProvider;
        }

        public async Task<dynamic> BuildFirstOrderViewModel()
        {
            dynamic vm = new ExpandoObject();

            var url = "http://localhost:20185/api/orders/first";
            var client = new HttpClient();
            var response = await client.GetAsync(url);

            vm.Order = await response.Content.AsExpandoAsync();
            vm.Customer = await this.orderCustomerInfoProvider.BuildCustomerViewModel(vm.Order.CustomerId);

            return vm;
        }
    }
}