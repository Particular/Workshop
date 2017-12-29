using System.Data.Entity;
using System.Threading.Tasks;
using NServiceBus;
using NServiceBus.Logging;
using Divergent.Customers.Data.Context;
using Divergent.Sales.Messages.Events;

namespace Divergent.Customers.Handlers
{
    public class OrderSubmittedHandler : IHandleMessages<OrderSubmittedEvent>
    {
        private static readonly ILog Log = LogManager.GetLogger<OrderSubmittedHandler>();

        public async Task Handle(OrderSubmittedEvent message, IMessageHandlerContext context)
        {
            Log.Info("Handling: OrderSubmittedEvent.");

            using (var db = new CustomersContext())
            {
                var customer = await db.Customers
                    .Include(c => c.Orders)
                    .SingleAsync(c => c.Id == message.CustomerId);

                customer.Orders.Add(new Data.Models.Order
                {
                    CustomerId = message.CustomerId,
                    OrderId = message.OrderId
                });

                await db.SaveChangesAsync();
            }
        }
    }
}
