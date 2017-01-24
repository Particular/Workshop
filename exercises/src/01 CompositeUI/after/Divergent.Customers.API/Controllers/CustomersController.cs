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

        [HttpGet, Route("byorders")]
        public async Task<IDictionary<Guid, Customer>> ByOrders(string orderIds)
        {
            var manyToManyRepo = new CustomerOrderRepository();

            var _orderIds = orderIds.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries)
                .Select(id => Guid.Parse(id))
                .ToList();

            var allCustomers = await _repository.Customers();
            var allManyToMany = await manyToManyRepo.CustomerOrderRelationships();

            var data = allManyToMany
                .Where( r=>_orderIds.Contains(r.OrderId))
                .Join(allCustomers, r => r.CustomerId, c => c.Id, (r, c) => 
                {
                    return new
                    {
                        Customer = c,
                        OrderId = r.OrderId
                    };
                })
                .ToDictionary(a=>a.OrderId, a=>a.Customer);

            return data;
        }
    }
}
