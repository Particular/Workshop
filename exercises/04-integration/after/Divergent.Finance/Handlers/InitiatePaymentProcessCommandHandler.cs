using Divergent.Finance.Messages.Events;
using Divergent.Finance.PaymentClient;
using NServiceBus;
using Divergent.Finance.Messages.Commands;

namespace Divergent.Finance.Handlers;

class InitiatePaymentProcessCommandHandler : IHandleMessages<InitiatePaymentProcessCommand>
{
    private readonly ReliablePaymentClient reliablePaymentClient;
    private readonly ILogger<InitiatePaymentProcessCommandHandler> logger;

    public InitiatePaymentProcessCommandHandler(ReliablePaymentClient reliablePaymentClient, ILogger<InitiatePaymentProcessCommandHandler> logger)
    {
        this.reliablePaymentClient = reliablePaymentClient;
        this.logger = logger;
    }

    public async Task Handle(InitiatePaymentProcessCommand message, IMessageHandlerContext context)
    {
        logger.LogInformation("Handle InitiatePaymentProcessCommand");

        await reliablePaymentClient.ProcessPayment(message.CustomerId, message.Amount);

        await context.Publish(new PaymentSucceededEvent
        {
            OrderId = message.OrderId,
        });
    }
}