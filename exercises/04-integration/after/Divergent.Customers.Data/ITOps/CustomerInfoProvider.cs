using System.Linq;
using System.Threading.Tasks;
using Divergent.Customers.Data.Context;
using Divergent.ITOps.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Divergent.Customers.Data.ITOps;

public class CustomerInfoProvider : IProvideCustomerInfo
{
    private readonly CustomersContext _db;

    public CustomerInfoProvider(CustomersContext db) => _db = db;

    public async Task<CustomerInfo> GetCustomerInfo(int customerId)
    {
        var customer = await _db.Customers.Where(c => c.Id == customerId).SingleAsync();

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