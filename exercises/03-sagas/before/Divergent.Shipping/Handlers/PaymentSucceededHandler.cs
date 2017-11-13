using System.Threading.Tasks;
using Divergent.Finance.Messages.Events;
using NServiceBus;
using NServiceBus.Logging;

namespace Divergent.Shipping.Handlers
{
    public class PaymentSucceededHandler : IHandleMessages<PaymentSucceededEvent>
    {
        private static readonly ILog Log = LogManager.GetLogger<PaymentSucceededHandler>();

        public async Task Handle(PaymentSucceededEvent message, IMessageHandlerContext context)
        {
            Log.Debug("Handle");

            // Store in database that order was successfully paid.
            // Look at all pending orders, paid and ready to be shipped, in batches to decide what to ship.

            await Task.CompletedTask;
        }
    }
}
