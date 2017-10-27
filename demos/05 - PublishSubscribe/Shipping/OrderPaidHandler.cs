using System.Threading.Tasks;
using NServiceBus;

namespace Shipping
{
    using System;
    using NServiceBus.Logging;
    using Shipping.Repositories;

    public class OrderPaidHandler : IHandleMessages<OrderPaid>
    {
        static ILog log = LogManager.GetLogger<OrderReceivedHandler>();
        OrderRepository orderRepository = new OrderRepository();

        public async Task Handle(OrderPaid message, IMessageHandlerContext context)
        {
            log.Info($"Shipping has received OrderReceived event with OrderId {message.OrderId}.");

            var order = await orderRepository.GetOrder(message.OrderId);

            if (order == null)
                throw new Exception("Order wasn't found!");

            order.IsPaid = true;
            await orderRepository.SaveOrder(order);
        }
    }
}
