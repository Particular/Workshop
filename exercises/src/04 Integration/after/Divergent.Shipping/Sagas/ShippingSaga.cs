using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Divergent.Finance.Messages.Events;
using Divergent.ITOps.Messages.Commands;
using Divergent.Sales.Messages.Events;
using NServiceBus;
using NServiceBus.Logging;

namespace Divergent.Shipping.Sagas
{
    class ShippingSaga : Saga<ShippingSagaData>,
        IAmStartedByMessages<OrderSubmittedEvent>,
        IAmStartedByMessages<PaymentSucceededEvent>
    {
        private static readonly ILog Log = LogManager.GetLogger<ShippingSaga>();

        protected override void ConfigureHowToFindSaga(SagaPropertyMapper<ShippingSagaData> mapper)
        {
            mapper.ConfigureMapping<OrderSubmittedEvent>(p => p.OrderId).ToSaga(s => s.OrderId);
            mapper.ConfigureMapping<PaymentSucceededEvent>(p => p.OrderId).ToSaga(s => s.OrderId);
        }

        public async Task Handle(OrderSubmittedEvent message, IMessageHandlerContext context)
        {
            Log.Info("Handle OrderSubmittedEvent");

            Data.IsOrderSubmitted = true;
            Data.OrderId = message.OrderId;
            Data.CustomerId = message.CustomerId;

            var products = from p in message.Products
                select new ShippingSagaData.Product {Identifier = p};
            Data.Products = products.ToList();

            await ProcessOrder(context);
        }

        public async Task Handle(PaymentSucceededEvent message, IMessageHandlerContext context)
        {
            Log.Info("Handle PaymentSucceededEvent");

            Data.OrderId = message.OrderId;
            Data.IsPaymentProcessedYet = true;
            await ProcessOrder(context);
        }

        private async Task ProcessOrder(IMessageHandlerContext context)
        {
            if (Data.IsOrderSubmitted && Data.IsPaymentProcessedYet)
            {
                await context.Send<ShipWithFedexCommand>(cmd =>
                {
                    cmd.OrderId = Data.OrderId;
                    cmd.CustomerId = Data.CustomerId;
                    cmd.Products = Data.Products.Select(s => s.Identifier).ToList();
                });

                MarkAsComplete();
            }
        }
    }
}
