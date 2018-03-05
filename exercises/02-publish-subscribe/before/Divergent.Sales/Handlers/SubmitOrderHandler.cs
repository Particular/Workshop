using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Divergent.Sales.Data.Context;
using Divergent.Sales.Data.Models;
using Divergent.Sales.Messages.Commands;
using NServiceBus;
using NServiceBus.Logging;

namespace Divergent.Sales.Handlers
{
    public class SubmitOrderHandler : IHandleMessages<SubmitOrderCommand>
    {
        private static readonly ILog Log = LogManager.GetLogger<SubmitOrderHandler>();

        public async Task Handle(SubmitOrderCommand message, IMessageHandlerContext context)
        {
            using (var db = new SalesContext())
            {
                Log.Info("Handle SubmitOrderCommand");

                var items = new List<Item>();

                var products = db.Products.ToList();

                message.Products.ForEach(p => items.Add(new Item
                {
                    Product = products.Single(s => s.Id == p)
                }));

                var order = new Data.Models.Order
                {
                    CustomerId = message.CustomerId,
                    DateTimeUtc = DateTime.UtcNow,
                    Items = items,
                    State = "New"
                };

                db.Orders.Add(order);
                await db.SaveChangesAsync();
            }
        }
    }
}
