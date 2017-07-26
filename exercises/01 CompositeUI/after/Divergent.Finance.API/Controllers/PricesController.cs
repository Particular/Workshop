using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using Divergent.Finance.Data.Repositories;
using System.Collections.Generic;

namespace Divergent.Finance.API.Controllers
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

        [HttpGet, Route("orders/total")]
        public async Task<IDictionary<Guid, double>> GetOrdersTotal(string orderIds)
        {
            var _ids = orderIds.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries)
                   .Select(id => Guid.Parse(id))
                   .ToList();

            var repo = new OrdersTotalRepository();
            var prices = (await repo.OrderTotalPrices())
                .Where(otp=>_ids.Contains(otp.OrderId))
                .ToDictionary(otp=>otp.OrderId, otp=>otp.TotalPrice);
            
            return prices;
        }
    }
}
