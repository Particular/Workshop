using System.Threading.Tasks;
using NServiceBus;
using NServiceBus.Logging;
using Divergent.Finance.Messages.Events;

namespace Divergent.Shipping.Handlers
{
    public class PaymentSucceededHandler : IHandleMessages<PaymentSucceededEvent>
    {
        private static readonly ILog Log = LogManager.GetLogger<PaymentSucceededHandler>();

        public Task Handle(PaymentSucceededEvent message, IMessageHandlerContext context)
        {
            Log.Info("Handle");

            return Task.CompletedTask;
            // Store in database that payment succeeded.
            // The order incl. products should also already have arrived and stored in database as well.
            //
            // When orders are paid before 12am, they will be shipped and arrive the next business day.

            await Task.CompletedTask;
        }
    }
}
