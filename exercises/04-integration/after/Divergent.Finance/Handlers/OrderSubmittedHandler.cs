using Divergent.Sales.Messages.Events;
using NServiceBus;
using Divergent.Finance.Data.Context;
using Divergent.Finance.Data.Models;
using Divergent.Finance.Messages.Commands;

namespace Divergent.Finance.Handlers;

public class OrderSubmittedHandler : IHandleMessages<OrderSubmittedEvent>
{
    private readonly FinanceContext _db;
    private readonly ILogger<OrderSubmittedHandler> _logger;

    public OrderSubmittedHandler(FinanceContext db, ILogger<OrderSubmittedHandler> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task Handle(OrderSubmittedEvent message, IMessageHandlerContext context)
    {
        _logger.LogInformation("Handle OrderSubmittedEvent");

        double amount = 0;

        var query = from price in _db.Prices
            where message.Products.Contains(price.ProductId)
            select price;

        foreach (var price in query)
        {
            var op = new OrderItemPrice
            {
                OrderId = message.OrderId,
                ItemPrice = price.ItemPrice,
                ProductId = price.ProductId
            };

            amount += price.ItemPrice;

            await _db.OrderItemPrices.AddAsync(op);
        }

        await _db.SaveChangesAsync();

        await context.SendLocal(new InitiatePaymentProcessCommand
        {
            CustomerId = message.CustomerId,
            OrderId = message.OrderId,
            Amount = amount
        });
    }
}