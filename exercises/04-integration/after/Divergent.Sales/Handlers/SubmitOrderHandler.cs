using System.Net.Http.Headers;
using System.Net.NetworkInformation;
using Divergent.Sales.Data.Models;
using Divergent.Sales.Messages.Commands;
using Divergent.Sales.Messages.Events;
using ITOps.EndpointConfig;
using LiteDB;
using NServiceBus;

namespace Divergent.Sales.Handlers;

public class SubmitOrderHandler : IHandleMessages<SubmitOrderCommand>
{
    private readonly ILiteDbContext db;
    private readonly ILogger<SubmitOrderHandler> log;

    public SubmitOrderHandler(ILiteDbContext db, ILogger<SubmitOrderHandler> log)
    {
        this.db = db;
        this.log = log;
    }

    public async Task Handle(SubmitOrderCommand message, IMessageHandlerContext context)
    {
        log.LogInformation("Handle SubmitOrderCommand");

        var orders = db.Database.GetCollection<Order>();

        var order = new Order
        {
            CustomerId = message.CustomerId,
            DateTimeUtc = DateTime.UtcNow,
            Items = message.Products,
            State = "New"
        };

        orders.Insert(order);

        // Publish event
        await context.Publish(new OrderSubmittedEvent
        {
            OrderId = order.Id,
            CustomerId = message.CustomerId,
            Products = message.Products,
        });
    }
}