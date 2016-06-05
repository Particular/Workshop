using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Logging;
using Divergent.Finance.Messages.Events;
using NServiceBus;

namespace Divergent.Shipping.Handlers
{
    public class PaymentSucceededHandler : IHandleMessages<PaymentSucceededEvent>
    {
        private static readonly ILog Log = LogManager.GetLogger<PaymentSucceededHandler>();

        public async Task Handle(PaymentSucceededEvent message, IMessageHandlerContext context)
        {
            Log.Debug("Handle");

            // Store in database that payment succeeded.
            // The order incl. products should also already have arrived and stored in database as well.
            //
            // When orders are paid before 12am, they will be shipped and arrive the next business day.
        }
    }
}
