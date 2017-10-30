using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Messages.Events;
using NServiceBus;
using NServiceBus.Logging;
using Shared;

public class OrderReceivedHandler : IHandleMessages<OrderReceived>
{
    readonly IEnumerable<Guid> priorityCustomers = Customers.GetPriorityCustomers();
    static ILog log = LogManager.GetLogger<OrderReceivedHandler>();

    public async Task Handle(OrderReceived message, IMessageHandlerContext context)
    {
        if (priorityCustomers.Contains(message.CustomerId))
        {
            log.Info($"!! Ignoring priority customer : {message.CustomerId}");
            return;
        }

        log.Info($"Received order {message.OrderId} for customer {message.CustomerId}.");
        await Task.Delay(1000);
    }
}