using Divergent.Finance.Messages.Events;
using Divergent.Sales.Messages.Events;
using NServiceBus;
using NServiceBus.Logging;

namespace Divergent.Shipping.Sagas;

class ShippingSaga : Saga<ShippingSagaData>,
    IAmStartedByMessages<OrderSubmittedEvent>,
    IAmStartedByMessages<PaymentSucceededEvent>
{
    private static readonly ILog Log = LogManager.GetLogger<ShippingSaga>();

    protected override void ConfigureHowToFindSaga(SagaPropertyMapper<ShippingSagaData> mapper)
    {
        mapper.MapSaga(saga => saga.OrderId)
            .ToMessage<OrderSubmittedEvent>(p => p.OrderId)
            .ToMessage<PaymentSucceededEvent>(p => p.OrderId);
    }

    public Task Handle(OrderSubmittedEvent message, IMessageHandlerContext context)
    {
        Log.Info("Handle OrderSubmittedEvent");

        Data.IsOrderSubmitted = true;
        Data.CustomerId = message.CustomerId;

        var projection = message.Products.Select(p => new ShippingSagaData.Product { Identifier = p });
        Data.Products = projection.ToList();

        return ProcessOrder(context);
    }

    public Task Handle(PaymentSucceededEvent message, IMessageHandlerContext context)
    {
        Log.Info("Handle PaymentSucceededEvent");

        Data.IsPaymentProcessed = true;
        return ProcessOrder(context);
    }

    private async Task ProcessOrder(IMessageHandlerContext context)
    {
        if (Data.IsOrderSubmitted && Data.IsPaymentProcessed)
        {
            await Task.CompletedTask; // Send a message to execute shipment
            MarkAsComplete();
        }
    }
}