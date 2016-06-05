using System.Threading.Tasks;
using Common.Logging;
using Divergent.Sales.Messages.Events;
using NServiceBus;

namespace Divergent.Shipping.Handlers
{
    public class OrderSubmittedHandler : IHandleMessages<OrderSubmittedEvent>
    {
        private static readonly ILog Log = LogManager.GetLogger<OrderSubmittedHandler>();

        public async Task Handle(OrderSubmittedEvent message, IMessageHandlerContext context)
        {
            Log.Info("Handle");
         
            // Store in database that order was submitted and which products belong to it.
            // If payment succeeds, we store that as well.
            //
            // When orders are paid before 12am, they will be shipped and arrive the next business day.
        }
    }
}
