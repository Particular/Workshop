using System.Threading.Tasks;

namespace Divergent.ITOps.Interfaces
{
    public interface IProvideCustomerInfo
    {
        CustomerInfo GetCustomerInfo(int customerId);
    }
}