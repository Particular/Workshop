using Divergent.Sales.Data.Models;
using Divergent.Sales.Messages.Commands;
using ITOps.EndpointConfig;
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

    public Task Handle(SubmitOrderCommand message, IMessageHandlerContext context)
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

        // Publish event and remove line 36
        return Task.CompletedTask;
    }
}