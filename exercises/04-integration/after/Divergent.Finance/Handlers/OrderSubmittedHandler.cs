using Divergent.Sales.Messages.Events;
using NServiceBus;
using Divergent.Finance.Data.Models;
using Divergent.Finance.Messages.Commands;
using ITOps.EndpointConfig;

namespace Divergent.Finance.Handlers;

public class OrderSubmittedHandler : IHandleMessages<OrderSubmittedEvent>
{
    private readonly ILiteDbContext db;
    private readonly ILogger<OrderSubmittedHandler> logger;

    public OrderSubmittedHandler(ILiteDbContext db, ILogger<OrderSubmittedHandler> logger)
    {
        this.db = db;
        this.logger = logger;
    }

    public async Task Handle(OrderSubmittedEvent message, IMessageHandlerContext context)
    {
        logger.LogInformation("Handle OrderSubmittedEvent");

        double amount = 0;

        var prices = db.Database.GetCollection<Price>();
        var orderItemPrices = db.Database.GetCollection<OrderItemPrice>();
        
        var query = from price in prices.Query()
            where message.Products.Contains(price.ProductId)
            select price;

        foreach (var price in query.ToList())
        {
            var op = new OrderItemPrice
            {
                OrderId = message.OrderId,
                ItemPrice = price.ItemPrice,
                ProductId = price.ProductId
            };

            amount += price.ItemPrice;

            orderItemPrices.Insert(op);
        }

        await context.SendLocal(new InitiatePaymentProcessCommand
        {
            CustomerId = message.CustomerId,
            OrderId = message.OrderId,
            Amount = amount
        });
    }
}