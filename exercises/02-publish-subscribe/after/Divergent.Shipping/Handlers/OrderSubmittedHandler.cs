using System.Data.Entity;
using System.Threading.Tasks;
using Divergent.Sales.Messages.Events;
using Divergent.Shipping.Data.Context;
using Divergent.Shipping.Data.Models;
using NServiceBus;
using NServiceBus.Logging;

namespace Divergent.Shipping.Handlers
{
    public class OrderSubmittedHandler : IHandleMessages<OrderSubmittedEvent>
    {
        private static readonly ILog Log = LogManager.GetLogger<OrderSubmittedHandler>();

        public async Task Handle(OrderSubmittedEvent message, IMessageHandlerContext context)
        {
            using (var db = new ShippingContext())
            {
                var shipment = await db.Shipments.FirstOrDefaultAsync(x => x.OrderId == message.OrderId);
                if (shipment == null)
                {
                    shipment = new Shipment
                    {
                        OrderId = message.OrderId
                    };
                    db.Shipments.Add(shipment);
                }
                shipment.IsOrderSubmitted = true;
                await db.SaveChangesAsync().ConfigureAwait(false);
            }
        }
    }
}
