using System.Threading.Tasks;
using System.Data.Entity;
using System.Linq;
using NServiceBus.Logging;
using Divergent.Sales.Messages.Events;
using Divergent.Customers.Data.Context;
using Divergent.Customers.Data.Models;

namespace Divergent.Customers.Handlers
{
    public class OrderSubmittedHandler : NServiceBus.IHandleMessages<OrderSubmittedEvent>
    {
        private static readonly ILog Log = LogManager.GetLogger<OrderSubmittedHandler>();

        public async Task Handle(OrderSubmittedEvent message, NServiceBus.IMessageHandlerContext context)
        {
            Log.Info("Handling: OrderSubmittedEvent.");

            using (var db = new CustomersContext())
            {
                var customer = db.Customers
                    .Include(c=>c.Orders)
                    .Where(c=>c.Id == message.CustomerId)
                    .Single();

                customer.Orders.Add(new Order
                {
                    CustomerId = message.CustomerId,
                    OrderId = message.OrderId
                });

                await db.SaveChangesAsync();
            }
        }
    }
}
