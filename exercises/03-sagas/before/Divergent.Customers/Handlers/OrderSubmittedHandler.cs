using Divergent.Sales.Messages.Events;
using Divergent.Customers.Data.Models;
using ITOps.EndpointConfig;

namespace Divergent.Customers.Handlers;

public class OrderSubmittedHandler : NServiceBus.IHandleMessages<OrderSubmittedEvent>
{
    private readonly ILiteDbContext db;
    private readonly ILogger<OrderSubmittedHandler> log;

    public OrderSubmittedHandler(ILiteDbContext db, ILogger<OrderSubmittedHandler> log)
    {
        this.db = db;
        this.log = log;
    }

    public Task Handle(OrderSubmittedEvent message, NServiceBus.IMessageHandlerContext context)
    {
        log.LogInformation($"Handling: {nameof(OrderSubmittedEvent)}");

        var orders = db.Database.GetCollection<Order>();

        orders.Insert(new Order
        {
            CustomerId = message.CustomerId,
            OrderId = message.OrderId
        });
        
        return Task.CompletedTask;
    }
}