using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Divergent.Shipping.Data.Context;
using NServiceBus;
using NServiceBus.Logging;

namespace Divergent.Shipping.BatchJobs
{
    public class ShippingBatchJob : IWantToRunWhenEndpointStartsAndStops
    {
        public Task Start(IMessageSession session)
        {
            return session.ScheduleEvery(TimeSpan.FromSeconds(10), "ShippingBatchJob", Execute);
        }

        async Task Execute(IPipelineContext pipelineContext)
        {
            using (var db = new ShippingContext())
            {
                var readyToShip = await db.Shipments.Where(x => !x.IsShippedYet && x.IsOrderSubmitted && x.IsPaymentProcessedYet)
                    .ToListAsync()
                    .ConfigureAwait(false);

                foreach (var shipment in readyToShip)
                {
                    shipment.IsShippedYet = true;
                    Log.Info($"Order {shipment.Id} shipped.");
                }

                await db.SaveChangesAsync().ConfigureAwait(false);
            }
        }

        public Task Stop(IMessageSession session)
        {
           return Task.CompletedTask;
        }

        private static readonly ILog Log = LogManager.GetLogger<ShippingBatchJob>();
    }
}