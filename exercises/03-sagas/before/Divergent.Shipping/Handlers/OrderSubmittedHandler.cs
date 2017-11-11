using System.Threading.Tasks;
using Divergent.Sales.Messages.Events;
using NServiceBus;
using NServiceBus.Logging;

namespace Divergent.Shipping.Handlers
{
    public class OrderSubmittedHandler : IHandleMessages<OrderSubmittedEvent>
    {
        private static readonly ILog Log = LogManager.GetLogger<OrderSubmittedHandler>();

        public async Task Handle(OrderSubmittedEvent message, IMessageHandlerContext context)
        {
            Log.Info("Handle");

            // Store in database that order was submitted and which products belong to it.
            // Look at all pending orders, paid and ready to be shipped, in batches to decide what to ship.

            await Task.CompletedTask;
        }
    }
}
