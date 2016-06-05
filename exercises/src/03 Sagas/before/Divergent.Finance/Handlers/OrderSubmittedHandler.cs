using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Divergent.Finance.Data.Repositories;
using Divergent.Finance.Messages.Events;
using Divergent.Finance.PaymentClient;
using Divergent.Sales.Messages.Events;
using NServiceBus;
using NServiceBus.Logging;

namespace Divergent.Finance.Handlers
{
    public class OrderSubmittedHandler : IHandleMessages<OrderSubmittedEvent>
    {
        private readonly IFinanceRepository _repository;
        private readonly ReliablePaymentClient _reliablePaymentClient;
        private static readonly ILog Log = LogManager.GetLogger<OrderSubmittedHandler>();

        public OrderSubmittedHandler(IFinanceRepository repository, ReliablePaymentClient reliablePaymentClient)
        {
            _repository = repository;
            _reliablePaymentClient = reliablePaymentClient;
        }

        public async Task Handle(OrderSubmittedEvent message, IMessageHandlerContext context)
        {
            Log.Info("Handle OrderSubmittedEvent");

            var amount = await GetAmount(message.Products);

            await _reliablePaymentClient.ProcessPayment(message.CustomerId, amount);

            await context.Publish<PaymentSucceededEvent>(e =>
            {
                e.OrderId = message.OrderId;
            });
        }

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
