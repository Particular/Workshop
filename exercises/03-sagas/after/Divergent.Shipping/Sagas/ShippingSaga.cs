using System.Linq;
using System.Threading.Tasks;
using Divergent.Finance.Messages.Events;
using Divergent.Sales.Messages.Events;
using NServiceBus;
using NServiceBus.Logging;
using NServiceBus.Persistence.Sql;

namespace Divergent.Shipping.Sagas
{
    class ShippingSaga : SqlSaga<ShippingSagaData>,
        IAmStartedByMessages<OrderSubmittedEvent>,
        IAmStartedByMessages<PaymentSucceededEvent>
    {
        private static readonly ILog Log = LogManager.GetLogger<ShippingSaga>();

        protected override string CorrelationPropertyName => nameof(ShippingSagaData.OrderId);

        protected override void ConfigureMapping(IMessagePropertyMapper mapper)
        {
            mapper.ConfigureMapping<OrderSubmittedEvent>(m => m.OrderId);
            mapper.ConfigureMapping<PaymentSucceededEvent>(m => m.OrderId);
        }

        public Task Handle(OrderSubmittedEvent message, IMessageHandlerContext context)
        {
            Log.Info("Handle OrderSubmittedEvent");

            Data.IsOrderSubmitted = true;
            Data.OrderId = message.OrderId;
            Data.CustomerId = message.CustomerId;

            var projection = message.Products.Select(p => new ShippingSagaData.Product { Identifier = p });
            Data.Products = projection.ToList();

            return ProcessOrder(context);
        }

        public Task Handle(PaymentSucceededEvent message, IMessageHandlerContext context)
        {
            Log.Info("Handle PaymentSucceededEvent");

            Data.OrderId = message.OrderId;
            Data.IsPaymentProcessed = true;

            return ProcessOrder(context);
        }

        private async Task ProcessOrder(IMessageHandlerContext context)
        {
            if (Data.IsOrderSubmitted && Data.IsPaymentProcessed)
            {
                await Task.CompletedTask; // Send a message to execute shipment
                MarkAsComplete();
            }
        }
    }
}
