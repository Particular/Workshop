using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Divergent.Finance.Data.Models;

namespace Divergent.Finance.Data.Repositories
{
    public interface IFinanceRepository
    {
        Task<Price> Price(Guid productId);
        Task<List<Price>> Prices();
    }
}