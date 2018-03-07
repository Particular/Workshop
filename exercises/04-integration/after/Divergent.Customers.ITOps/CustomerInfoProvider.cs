using System.Linq;
using System.Threading.Tasks;
using Divergent.ITOps.Interfaces;

namespace Divergent.Customers.ITOps
{
    public class CustomerInfoProvider : IProvideCustomerInfo
    {
        public Task<CustomerInfo> GetCustomerInfo(int customerId)
        {
            using (var db = new CustomersContext())
            {
                var customer = db.Customers.Where(c => c.Id == customerId).Single();

                return Task.FromResult(new CustomerInfo
                {
                    Name = customer.Name,
                    Street = customer.Street,
                    City = customer.City,
                    PostalCode = customer.PostalCode,
                    Country = customer.Country,
                });
            }
        }
    }
}
