using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Divergent.Sales.Data.Context;
using Divergent.Sales.Data.Models;
using Divergent.Sales.Messages.Commands;
using Divergent.Sales.Messages.Events;
using NServiceBus;
using NServiceBus.Logging;

namespace Divergent.Sales.Handlers
{
    public class SubmitOrderHandler : IHandleMessages<SubmitOrderCommand>
    {
        private readonly ISalesContext _context;
        private static readonly ILog Log = LogManager.GetLogger<SubmitOrderHandler>();

        public SubmitOrderHandler(ISalesContext context)
        {
            _context = context;
        }

        public async Task Handle(SubmitOrderCommand message, IMessageHandlerContext context)
        {
            Log.Info("Handle SubmitOrderCommand");

            var items = new List<Item>();

            var products = _context.Products.ToList();

            message.Products.ForEach(p => items.Add(new Item()
            {
                Id = Guid.NewGuid(),
                OrderId = message.OrderId,
                Product = products.Single(s => s.Id == p)
            }));

            var order = new Divergent.Sales.Data.Models.Order()
            {
                CustomerId = message.CustomerId,
                DateTimeUtc = DateTime.UtcNow,
                Id = message.OrderId,
                Items = items,
                State = "New"
            };

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            // Publish event
            await context.Publish<OrderSubmittedEvent>(e =>
            {
                e.OrderId = message.OrderId;
                e.CustomerId = message.CustomerId;
                e.Products = message.Products;
            });
        }
    }
}
