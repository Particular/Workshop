using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Divergent.Finance.Data.Repositories;
using Divergent.Finance.PaymentClient;
using NServiceBus.Logging;

namespace Divergent.Finance.Handlers
{
    public class OrderSubmittedHandler //TODO: implement IHandleMessages<OrderSubmittedEvent>
    {
        private readonly IFinanceRepository _repository;
        private readonly ReliablePaymentClient _reliablePaymentClient;
        private static readonly ILog Log = LogManager.GetLogger<OrderSubmittedHandler>();

        public OrderSubmittedHandler(IFinanceRepository repository, ReliablePaymentClient reliablePaymentClient)
        {
            _repository = repository;
            _reliablePaymentClient = reliablePaymentClient;
        }

        //TODO: implement IHandleMessages<OrderSubmittedEvent>

        private async Task<double> GetAmount(List<Guid> products)
        {
            var prices = await _repository.Prices();

            var query = from p in prices
                        where products.Contains(p.ProductId)
                        select p;

            double amount = query.Select(p => p.ItemPrice).DefaultIfEmpty(0d).Sum();

            return amount;
        }
    }
}
