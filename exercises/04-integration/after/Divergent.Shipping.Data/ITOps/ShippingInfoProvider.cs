using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Divergent.ITOps.Interfaces;

namespace Divergent.Shipping.Data.ITOps
{
    public class ShippingInfoProvider : IProvideShippingInfo
    {
        public Task<PackageInfo> GetPackageInfo(IEnumerable<int> productIds)
        {
            var count = productIds.Count();

            return Task.FromResult(new PackageInfo
            {
                Weight = WeightCalculator.CalculateWeight(count),
                Volume = VolumeEstimator.Calculate(count)
            });
        }
    }
}
