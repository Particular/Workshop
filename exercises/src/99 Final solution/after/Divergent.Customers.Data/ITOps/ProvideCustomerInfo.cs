using System;
using System.Threading.Tasks;
using Divergent.Customers.Data.Repositories;
using Divergent.ITOps.Interfaces;

namespace Divergent.Customers.Data.ITOps
{
    public class CustomerInfoProvider : IProvideCustomerInfo
    {
        private readonly ICustomerRepository _repository = new CustomerRepository();

        public async Task<CustomerInfo> GetCustomerInfo(Guid customerId)
        {
            var customer = await _repository.Customer(customerId);
            
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
