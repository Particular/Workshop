using System.Threading.Tasks;
using NServiceBus;
using NServiceBus.Logging;

public class OrderReceivedHandler : IHandleMessages<OrderReceived>
{
    static ILog log = LogManager.GetLogger<OrderReceivedHandler>();

    public async Task Handle(OrderReceived message, IMessageHandlerContext context)
    {
        log.Info($"Finance has received OrderReceived event with OrderId {message.OrderId}.");

        await Task.Delay(1000);
        log.Info($"Finance succesfully debitted OrderId {message.OrderId}");

        await context.Publish(new OrderPaid()
        {
            OrderId = message.OrderId
        });
    }
}