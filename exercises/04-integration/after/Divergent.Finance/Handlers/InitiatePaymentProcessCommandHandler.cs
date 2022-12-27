using Divergent.Finance.Messages.Events;
using Divergent.Finance.PaymentClient;
using NServiceBus;
using Divergent.Finance.Messages.Commands;

namespace Divergent.Finance.Handlers;

class InitiatePaymentProcessCommandHandler : IHandleMessages<InitiatePaymentProcessCommand>
{
    private readonly ReliablePaymentClient _reliablePaymentClient;
    private readonly ILogger<InitiatePaymentProcessCommandHandler> _logger;

    public InitiatePaymentProcessCommandHandler(ReliablePaymentClient reliablePaymentClient, ILogger<InitiatePaymentProcessCommandHandler> logger)
    {
        _reliablePaymentClient = reliablePaymentClient;
        _logger = logger;
    }

    public async Task Handle(InitiatePaymentProcessCommand message, IMessageHandlerContext context)
    {
        _logger.LogInformation("Handle InitiatePaymentProcessCommand");

        await _reliablePaymentClient.ProcessPayment(message.CustomerId, message.Amount);

        await context.Publish(new PaymentSucceededEvent
        {
            OrderId = message.OrderId,
        });
    }
}