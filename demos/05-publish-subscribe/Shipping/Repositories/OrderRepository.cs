namespace Shipping.Repositories
{
    using System;
    using System.Threading.Tasks;
    using Shipping.Entities;

    class OrderRepository
    {
        public async Task SaveOrder(Order order)
        {
            await Task.Delay(1);
        }

        public async Task<Order> GetOrder(Guid orderId)
        {
            await Task.Delay(1);
            return new Order()
            {
                OrderId = orderId
            };
        }
    }
}
