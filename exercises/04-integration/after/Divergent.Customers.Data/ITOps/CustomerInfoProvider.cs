using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Divergent.ITOps.Interfaces;

namespace Divergent.Customers.Data.ITOps
{
    public class CustomerInfoProvider : IProvideCustomerInfo
    {
        public async Task<CustomerInfo> GetCustomerInfo(int customerId)
        {
            using (var db = new Context.CustomersContext())
            {
                var customer = await db.Customers.Where(c => c.Id == customerId).SingleAsync();

                return new CustomerInfo
                {
                    Name = customer.Name,
                    Street = customer.Street,
                    City = customer.City,
                    PostalCode = customer.PostalCode,
                    Country = customer.Country,
                };
            }
        }
    }
}
