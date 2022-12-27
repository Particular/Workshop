using Divergent.Sales.Data.Context;
using Divergent.Sales.Data.Models;
using Divergent.Sales.Messages.Commands;
using Divergent.Sales.Messages.Events;
using NServiceBus;

namespace Divergent.Sales.Handlers;

public class SubmitOrderHandler : IHandleMessages<SubmitOrderCommand>
{
    private readonly SalesContext _db;
    private readonly ILogger<SubmitOrderHandler> _log;

    public SubmitOrderHandler(SalesContext db, ILogger<SubmitOrderHandler> log)
    {
        _db = db;
        _log = log;
    }

    public async Task Handle(SubmitOrderCommand message, IMessageHandlerContext context)
    {
        _log.LogInformation("Handle SubmitOrderCommand");

        var items = new List<Item>();

        var products = _db.Products.ToList();

        message.Products.ForEach(p => items.Add(new Item
        {
            Product = products.Single(s => s.Id == p)
        }));

        var order = new Order
        {
            CustomerId = message.CustomerId,
            DateTimeUtc = DateTime.UtcNow,
            Items = items,
            State = "New"
        };

        await _db.Orders.AddAsync(order, context.CancellationToken);
        await _db.SaveChangesAsync(context.CancellationToken);

        // Publish event
        await context.Publish(new OrderSubmittedEvent
        {
            OrderId = order.Id,
            CustomerId = message.CustomerId,
            Products = message.Products,
        });
    }
}