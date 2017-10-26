using System.Collections.Generic;
using System.Threading.Tasks;

namespace Divergent.Sales.Data.Repositories
{
    using Models;

    public interface IOrderRepository
    {
        Task<List<Order>> Orders();
    }
}