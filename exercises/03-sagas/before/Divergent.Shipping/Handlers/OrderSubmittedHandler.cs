using Divergent.Sales.Messages.Events;
using NServiceBus;

namespace Divergent.Shipping.Handlers;

public class OrderSubmittedHandler : IHandleMessages<OrderSubmittedEvent>
{
    readonly ILogger<OrderSubmittedHandler> logger;

    public OrderSubmittedHandler(ILogger<OrderSubmittedHandler> logger)
    {
        this.logger = logger;
    }
    
    public async Task Handle(OrderSubmittedEvent message, IMessageHandlerContext context)
    {
        logger.LogDebug("Handle");

        // Store in database that order was submitted and which products belong to it.
        // Look at all pending orders, paid and ready to be shipped, in batches to decide what to ship.

        await Task.CompletedTask;
    }
}