using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Divergent.Finance.Data.Models;

namespace Divergent.Finance.Data.Repositories
{
    public class OrdersTotalRepository
    {
        public Task<List<OrderTotalPrice>> OrderTotalPrices()
        {
            return Task.FromResult(SeedOrderTotalPrices());
        }

        private List<OrderTotalPrice> SeedOrderTotalPrices()
        {
            var particularOrderId = Guid.Parse("6c3945c9-0b64-414a-9e5f-892de482ae2b");
            var nserivceBusOrderId = Guid.Parse("41f74caf-2ef9-46ca-9445-2fcdf35751fb");

            return new List<OrderTotalPrice>
            {
                new OrderTotalPrice { OrderId = particularOrderId, TotalPrice = 120 },
                new OrderTotalPrice { OrderId= nserivceBusOrderId, TotalPrice = 90 }
            };
        }
    }
}
