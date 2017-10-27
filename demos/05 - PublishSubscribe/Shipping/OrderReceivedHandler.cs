using System.Threading.Tasks;
using NServiceBus;
using NServiceBus.Logging;
using Shipping.Repositories;

public class OrderReceivedHandler : IHandleMessages<OrderReceived>
{
    static ILog log = LogManager.GetLogger<OrderReceivedHandler>();

    public async Task Handle(OrderReceived message, IMessageHandlerContext context)
    {
        log.Info($"Shipping has received OrderReceived event with OrderId {message.OrderId}.");

        var order = new Shipping.Entities.Order
        {
            OrderId = message.OrderId
        };

        await new OrderRepository().SaveOrder(order);
    }
}