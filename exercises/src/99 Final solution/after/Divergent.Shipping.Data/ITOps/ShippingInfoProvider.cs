using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Divergent.ITOps.Interfaces;

namespace Divergent.Shipping.Data.ITOps
{
    public class ShippingInfoProvider : IProvideShippingInfo
    {
        public Task<PackageInfo> GetPackageInfo(List<Guid> productIds)
        {
            return Task.FromResult(new PackageInfo()
            {
                Weight = WeightCalculator.CalculateWeight(productIds.Count),
                Volume = VolumeEstimator.Calculate(productIds.Count)
            });
        }
    }
}
