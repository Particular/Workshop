using System.Linq;
using System.Threading.Tasks;
using Divergent.Sales.Messages.Events;
using NServiceBus;
using NServiceBus.Logging;
using Divergent.Finance.Data.Context;
using Divergent.Finance.Data.Models;
using Divergent.Finance.Messages.Commands;

namespace Divergent.Finance.Handlers
{
    public class OrderSubmittedHandler : IHandleMessages<OrderSubmittedEvent>
    {
        private static readonly ILog Log = LogManager.GetLogger<OrderSubmittedHandler>();

        public async Task Handle(OrderSubmittedEvent message, IMessageHandlerContext context)
        {
            Log.Info("Handle OrderSubmittedEvent");

            double amount = 0;
            using (var db = new FinanceContext())
            {
                var query = from price in db.Prices
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

                    db.OrderItemPrices.Add(op);
                }

                await db.SaveChangesAsync();
            }

            await context.SendLocal(new InitiatePaymentProcessCommand
            {
                CustomerId = message.CustomerId,
                OrderId = message.OrderId,
                Amount = amount
            });
        }
    }
}
