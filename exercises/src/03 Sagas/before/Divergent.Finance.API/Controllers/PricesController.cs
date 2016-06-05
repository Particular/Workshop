using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using Divergent.Finance.Data.Repositories;

namespace Finance.API.Controllers
{
    [RoutePrefix("api/prices")]
    public class PricingController : ApiController
    {
        private readonly IFinanceRepository _financeRepository;

        public PricingController(IFinanceRepository financeRepository)
        {
            _financeRepository = financeRepository;
        }

        [HttpGet, Route("total/{productIds}")]
        public async Task<dynamic> GetTotal(string productIds)
        {
            var _ids = productIds.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries)
                   .Select(id => Guid.Parse(id))
                   .ToList();

            var prices = await _financeRepository.Prices();

            return _ids.Sum(productId
                        => prices.Single(s => s.ProductId == productId).ItemPrice);
        }
    }
}
