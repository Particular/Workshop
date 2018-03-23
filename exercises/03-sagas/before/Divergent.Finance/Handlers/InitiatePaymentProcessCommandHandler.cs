using System.Threading.Tasks;
using Divergent.Finance.Messages.Events;
using Divergent.Finance.PaymentClient;
using NServiceBus;
using NServiceBus.Logging;
using Divergent.Finance.Messages.Commands;

namespace Divergent.Finance.Handlers
{
    class InitiatePaymentProcessCommandHandler : IHandleMessages<InitiatePaymentProcessCommand>
    {
        private static readonly ILog Log = LogManager.GetLogger<InitiatePaymentProcessCommand>();
        private readonly ReliablePaymentClient _reliablePaymentClient;

        public InitiatePaymentProcessCommandHandler(ReliablePaymentClient reliablePaymentClient)
        {
            _reliablePaymentClient = reliablePaymentClient;
        }

        public async Task Handle(InitiatePaymentProcessCommand message, IMessageHandlerContext context)
        {
            Log.Info("Handle InitiatePaymentProcessCommand");

            await _reliablePaymentClient.ProcessPayment(message.CustomerId, message.Amount);

            await context.Publish(new PaymentSucceededEvent
            {
                OrderId = message.OrderId,
            });
        }
    }
}
