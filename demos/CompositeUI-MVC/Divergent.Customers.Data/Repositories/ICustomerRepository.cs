using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Divergent.Customers.Data.Models;

namespace Divergent.Customers.Data.Repositories
{
    public interface ICustomerRepository
    {
        Task<Customer> Customer(Guid id);
        Task<List<Customer>> Customers();
    }
}