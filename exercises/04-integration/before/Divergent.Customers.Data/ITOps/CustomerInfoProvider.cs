using System.Threading.Tasks;
using Divergent.Customers.Data.Models;
using Divergent.ITOps.Interfaces;
using ITOps.EndpointConfig;

namespace Divergent.Customers.Data.ITOps;

public class CustomerInfoProvider : IProvideCustomerInfo
{
    private readonly ILiteDbContext db;

    public CustomerInfoProvider(ILiteDbContext db) => this.db = db;

    public CustomerInfo GetCustomerInfo(int customerId)
    {
        var customer = db.Database.GetCollection<Customer>().Query().Where(c => c.Id == customerId).First();

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