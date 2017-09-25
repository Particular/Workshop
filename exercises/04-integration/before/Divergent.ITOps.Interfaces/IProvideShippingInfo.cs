using System.Collections.Generic;
using System.Threading.Tasks;

namespace Divergent.ITOps.Interfaces
{
    public interface IProvideShippingInfo
    {
        Task<PackageInfo> GetPackageInfo(IEnumerable<int> productIds);
    }
}