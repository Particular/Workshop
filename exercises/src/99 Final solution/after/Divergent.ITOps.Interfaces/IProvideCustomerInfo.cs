using System;
using System.Threading.Tasks;

namespace Divergent.ITOps.Interfaces
{
    public interface IProvideCustomerInfo
    {
        Task<CustomerInfo> GetCustomerInfo(Guid customerId);
    }
}