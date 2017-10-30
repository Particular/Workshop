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

    public Task Handle(OrderReceived message, IMessageHandlerContext context)
    {
        if (!priorityCustomers.Contains(message.CustomerId))
        {
            log.Info($"!! Ignoring regular customer : {message.CustomerId}");
            return Task.CompletedTask;
        }

        log.Info($"Received order {message.OrderId} for customer {message.CustomerId}.");
        return Task.CompletedTask;
    }
}