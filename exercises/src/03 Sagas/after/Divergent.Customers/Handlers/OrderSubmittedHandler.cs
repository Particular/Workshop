using System.Threading.Tasks;
using Divergent.Sales.Messages.Events;
using NServiceBus.Logging;
using System.Linq;
using NServiceBus;
using Raven.Client;

namespace Divergent.Customers.Handlers
{
    public class OrderSubmittedHandler : IHandleMessages<OrderSubmittedEvent>
    {
        private static readonly ILog Log = LogManager.GetLogger<OrderSubmittedHandler>();

        public async Task Handle(OrderSubmittedEvent message, NServiceBus.IMessageHandlerContext context)
        {
            Log.Info("Handling: OrderSubmittedEvent.");

            var session = context.SynchronizedStorageSession.RavenSession();
            var customer = await session.Query<Data.Models.Customer>()
                    .Where(c => c.Id == message.CustomerId)
                    .SingleAsync();

            customer.Orders.Add(new Data.Models.Order()
            {
                CustomerId = message.CustomerId,
                OrderId = message.OrderId
            });
        }
    }
}