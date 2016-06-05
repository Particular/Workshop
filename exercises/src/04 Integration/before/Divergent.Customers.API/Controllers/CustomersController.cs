using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using Divergent.Customers.Data.Repositories;
using Divergent.Customers.Data.Models;

namespace Customers.API.Controllers
{
    [RoutePrefix("api/customers")]
    public class CustomersController : ApiController
    {
        private readonly ICustomerRepository _repository;

        public CustomersController(ICustomerRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public Task<Customer> Get(Guid id)
        {
            return _repository.Customer(id);
        }

        [HttpGet, Route("ByIds/{ids}")]
        public async Task<IEnumerable<Customer>> ByIds(string ids)
        {
            var _ids = ids.Split("|".ToCharArray(), StringSplitOptions.RemoveEmptyEntries)
                .Select(id=> Guid.Parse(id))
                .ToList();

            var customers = await _repository.Customers();

            return customers.Where(c => _ids.Contains(c.Id));
        }
    }
}
