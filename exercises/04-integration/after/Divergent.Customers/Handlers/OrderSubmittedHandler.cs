using Divergent.Sales.Messages.Events;
using Divergent.Customers.Data.Context;
using Divergent.Customers.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace Divergent.Customers.Handlers;

public class OrderSubmittedHandler : NServiceBus.IHandleMessages<OrderSubmittedEvent>
{
    private readonly CustomersContext _db;
    private readonly ILogger<OrderSubmittedHandler> _log;

    public OrderSubmittedHandler(CustomersContext db, ILogger<OrderSubmittedHandler> log)
    {
        _db = db;
        _log = log;
    }

    public async Task Handle(OrderSubmittedEvent message, NServiceBus.IMessageHandlerContext context)
    {
        _log.LogInformation($"Handling: {nameof(OrderSubmittedEvent)}");

        var customer = await _db.Customers
            .Include(c => c.Orders)
            .Where(c => c.Id == message.CustomerId)
            .SingleAsync(context.CancellationToken);

        customer.Orders.Add(new Order
        {
            CustomerId = message.CustomerId,
            OrderId = message.OrderId
        });

        await _db.SaveChangesAsync(context.CancellationToken);
    }
}