# Exercise 02 - Publish-Subscribe

In NServiceBus endpoints communicate by sending each other messages. In this exercise we'll focus on a specific type of messages - events. We will also explore a very powerful and popular messaging pattern - Publish/Subscribe (often also called Pub/Sub).

Events are used to communicate that some action has taken place, they're informing about a fact that occurred in the past. In Pub/Sub the sender (called Publisher) and the receiver (called Subscriber) are loosely coupled. There might be zero, one or multiple Subscribers interested in a specific event. In order to receive that event they need to explicitly subscribe to it. In NServiceBus the subscription request is sent by the framework, after specifying message mappings and implementing a handler which will process the event. The Publisher sends a copy of the event message to each subscriber.

## Introduction

In the last exercise you've extended the UI by showing additional information. As you probably noticed, in the Orders page we now have a button "Create new order". In this exercise we'll complete the process of placing a new order.

### Business requirement

When a customer creates a new order, that information is stored in a database. In order to complete the process, you need to provide the ability to pay for the placed order.

### What's provided for you:
- Have a look at the `EndpointConfig` class in the `Divergent.Finance` project. Note that we use conventions to specify which messages are events:

`conventions.DefiningEventsAs(t => t.Namespace != null && t.Namespace.StartsWith("Divergent") && t.Namespace.EndsWith("Events") && t.Name.EndsWith("Event"));`

If you create a class inside a namespace which name ends with "Events" and the name of this class ends with "Event", then NServiceBus will know it's an event.

- In the `Divergent.Finance` project inside `PaymentClient` directory you can find a provided implementation for handling payments called `ReliablePaymentClient`.


## Exercise 02.1 - Create and publish the `OrderSubmittedEvent`

In this exercise you'll create a new event called `OrderSubmittedEvent`. That event will be published by `SubmitOrderHandler` in the `Divergent.Sales` project.

**1)** Compile the application to restore all NuGet packages.

**2)** Have a look at the following classes: `Customer` in the `Divergent.Customers.Data.Models`, `Order` and `Product` in the `Divergent.Sales.Data.Models`.

**3)** In the `Divergent.Sales.Messages` project, in the directory `Events` add a new class called `OrderSubmittedEvent.cs`. The class should have three properties with public setters and getters: Id of the order, Id of the customer, and the list of product Ids.

```
namespace Divergent.Sales.Messages.Events
{
    public class OrderSubmittedEvent
    {
        public int OrderId { get; set; }
        public int CustomerId { get; set; }

        public List<int> Products { get; set; }
    }
}
```

**4)** Have a look at `SubmitOrderHandler` class in the `Divergent.Sales` project. At the end of the handler, after line 46, publish the `OrderSubmittedEvent`, by calling ```context.Publish<OrderSubmittedEvent>()``` method. Copy the properties from the incoming `SubmitOrderCommand` message, to the properties of the event.

```
await context.Publish<OrderSubmittedEvent>(e =>
{
    e.OrderId = order.Id;
    e.CustomerId = message.CustomerId;
    e.Products = message.Products;
});
```

## Exercise 02.2 - Handle the `OrderSubmittedEvent` in Shipping, Finance and Customers

In this exercise you'll handle the `OrderSubmittedEvent` by logging the information in the `OrderSubmittedHandler` class in `Divergent.Shipping` project and in the `OrderSubmittedHandler` class in `Divergent.Sales` project. Then we'll extend the handler implementation in the `Divergent.Finance` project in order to process the payment by using the provided `GetAmount()` method and `ReliablePaymentClient` class.

**1)** Create the `OrderSubmittedHandler` class in the `Divergent.Shipping` project, inside the `Handlers` namespace.

**2)** The `OrderSubmittedHandler` should process the `OrderSubmittedEvent` published by `Divergent.Sales`. In order to handle this event implement the `IHandleMessages<OrderSubmittedEvent>` interface in the `OrderSubmittedHandler` class.

**3)** In the `App.config` file in the `Divergent.Shipping` project, add a new configuration section called `UnicastBusConfig` and specify `MessageEndpointMappings` for the assembly containing `OrderSubmittedEvent` event. The route information provided in the mapping is used by NServiceBus internally to send subscription requests from the subscriber to the publisher.

```
<configSections>
  <section name="UnicastBusConfig" type="NServiceBus.Config.UnicastBusConfig, NServiceBus.Core" />
</configSections>
<UnicastBusConfig>
  <MessageEndpointMappings>
    <add Assembly="Divergent.Sales.Messages" Endpoint="Divergent.Sales" />
  </MessageEndpointMappings>
</UnicastBusConfig>
```

**4)** Use the provided logger to log information that the event was received and handled.

```
namespace Divergent.Shipping.Handlers
{
    public class OrderSubmittedHandler : IHandleMessages<OrderSubmittedEvent>
    {
        private static readonly ILog Log = LogManager.GetLogger<OrderSubmittedHandler>();

        public async Task Handle(OrderSubmittedEvent message, IMessageHandlerContext context)
        {
            Log.Info("Handle");
        }
    }
}
```

**5)** Create the `OrderSubmittedHandler` class in the `Divergent.Finance` project, inside the `Handlers` namespace.

**6)** The `OrderSubmittedHandler` should also process the `OrderSubmittedEvent` published by `Divergent.Sales`. In order to handle this implement the `IHandleMessages<OrderSubmittedEvent>` interface in the `OrderSubmittedHandler` class.

**7)** In the `App.config` file in the `Divergent.Finance` project, add a new configuration section called `UnicastBusConfig` and specify `MessageEndpointMappings` for the assembly containing `OrderSubmittedEvent` event.

```
<configSections>
  <section name="UnicastBusConfig" type="NServiceBus.Config.UnicastBusConfig, NServiceBus.Core" />
</configSections>

<UnicastBusConfig>
  <MessageEndpointMappings>
    <add Assembly="Divergent.Sales.Messages" Endpoint="Divergent.Sales" />
  </MessageEndpointMappings>
</UnicastBusConfig>
```

**8)** When Finance receives the `OrderSubmittedEvent` message it needs to keep track of item prices that belong to the submitted order. And finally initiate the payment process by sending the `InitiatePaymentProcessCommand` message.

```
namespace Divergent.Finance.Handlers
{
    public class OrderSubmittedHandler : IHandleMessages<OrderSubmittedEvent>
    {
        private static readonly ILog Log = LogManager.GetLogger<OrderSubmittedHandler>();

        public async Task Handle(OrderSubmittedEvent message, IMessageHandlerContext context)
        {
            Log.Info("Handle OrderSubmittedEvent");

            double amount = 0;
            using (var db = new FinanceContext())
            {
                var query = from price in db.Prices
                            where message.Products.Contains(price.ProductId)
                            select price;

                foreach (var price in query)
                {
                    var op = new OrderItemPrice()
                    {
                        OrderId = message.OrderId,
                        ItemPrice = price.ItemPrice,
                        ProductId = price.ProductId
                    };

                    amount += price.ItemPrice;

                    db.OrderItemPrices.Add(op);
                }

                await db.SaveChangesAsync();
            }

            await context.SendLocal(new InitiatePaymentProcessCommand()
            {
                CustomerId = message.CustomerId,
                OrderId = message.OrderId,
                Amount = amount
            });
        }
    }
}
```

**9)**  In the `Divergent.Finance` project create the `InitiatePaymentProcessCommandHandler` class inside the `Handlers` namespace in order to handle the payment process.

```
namespace Divergent.Finance.Handlers
{
    class InitiatePaymentProcessCommandHandler : IHandleMessages<InitiatePaymentProcessCommand>
    {
        private static readonly ILog Log = LogManager.GetLogger<InitiatePaymentProcessCommand>();
        private readonly ReliablePaymentClient _reliablePaymentClient;

        public InitiatePaymentProcessCommandHandler(ReliablePaymentClient reliablePaymentClient)
        {
            _reliablePaymentClient = reliablePaymentClient;
        }

        public async Task Handle(InitiatePaymentProcessCommand message, IMessageHandlerContext context)
        {
            Log.Info("Handle InitiatePaymentProcessCommand");

            await _reliablePaymentClient.ProcessPayment(message.CustomerId, message.Amount);
        }
    }
}
```

**10)** In the `Divergent.Customers` project create the `OrderSubmittedHandler` class inside the `Handlers` namespace in order to keep track of which orders have been submitted by which customer.

```
namespace Divergent.Customers.Handlers
{
    public class OrderSubmittedHandler : NServiceBus.IHandleMessages<OrderSubmittedEvent>
    {
        private static readonly ILog Log = LogManager.GetLogger<OrderSubmittedHandler>();
    
        public async Task Handle(OrderSubmittedEvent message, NServiceBus.IMessageHandlerContext context)
        {
            Log.Info("Handling: OrderSubmittedEvent.");
    
            using (var db = new CustomersContext())
            {
                var customer = await db.Customers
                    .Include(c=>c.Orders)
                    .SingleAsync(c=>c.Id == message.CustomerId);
    
                customer.Orders.Add(new Data.Models.Order()
                {
                    CustomerId = message.CustomerId,
                    OrderId = message.OrderId
                });
    
                await db.SaveChangesAsync();
            }
        }
    }
}
```

**11)** In the `App.config` file in the `Divergent.Customers` project add a new configuration section called `UnicastBusConfig` and specify `MessageEndpointMappings` for the assembly containing `OrderSubmittedEvent` event.

```
<configSections>
    <section name="UnicastBusConfig" type="NServiceBus.Config.UnicastBusConfig, NServiceBus.Core" />
</configSections>

<UnicastBusConfig>
  <MessageEndpointMappings>
    <add Assembly="Divergent.Sales.Messages" Endpoint="Divergent.Sales" />
  </MessageEndpointMappings>
</UnicastBusConfig>
```



## Exercise 02.3 - Create and publish the `PaymentSucceededEvent`

In this exercise we'll create a new event called `PaymentSucceededEvent`. This event will be published by the `InitiatePaymentProcessCommandHandler` in the `Divergent.Finance` project.

**1)** In the `Divergent.Finance.Messages` project, in the directory `Events`, add a new class called `PaymentSucceededEvent.cs`. The class should have only a single property with a public setter and a getter: id of the order.

```
namespace Divergent.Finance.Messages.Events
{
    public class PaymentSucceededEvent
    {
        public int OrderId { get; set; }
    }
}
```

**2)**  At the end of the `InitiatePaymentProcessCommandHandler` publish the `PaymentSucceededEvent` by calling `context.Publish<PaymentSucceededEvent>()` method. Copy the order id from the incoming `InitiatePaymentProcessCommand` message, to the property of the event.

```
public async Task Handle(InitiatePaymentProcessCommand message, IMessageHandlerContext context)
{
   Log.Info("Handle InitiatePaymentProcessCommand");

   await _reliablePaymentClient.ProcessPayment(message.CustomerId, message.Amount);
   await context.Publish<PaymentSucceededEvent>(e =>
   {
      e.OrderId = message.OrderId;
   });
}
```

## Exercise 02.4 - Handle the `PaymentSucceededEvent`

In this exercise we'll handle the `PaymentSucceededEvent` by logging the information in the `PaymentSucceededHandler` class in `Divergent.Shipping`.

**1)** Create the `PaymentSucceededHandler` class in the `Divergent.Shipping` project, in the `Handlers` namespace.

**2)** The `PaymentSucceededHandler` should process the `PaymentSucceededEvent` published by `Divergent.Finance`. In order to handle this event implement the `IHandleMessages<PaymentSucceededEvent>` interface in the `PaymentSucceededEvent` class.

**3)** Use the provided logger to log information that the event was received and handled.

```
namespace Divergent.Shipping.Handlers
{
    public class PaymentSucceededHandler : IHandleMessages<PaymentSucceededEvent>
    {
        private static readonly ILog Log = LogManager.GetLogger<PaymentSucceededHandler>();

        public async Task Handle(PaymentSucceededEvent message, IMessageHandlerContext context)
        {
            Log.Debug("Handle");
        }
    }
}
```

**4)** In the `App.config` file in the `Divergent.Shipping` project add `MessageEndpointMappings` for the assembly containing `PaymentSucceededEvent` event.
```
<UnicastBusConfig>
  <MessageEndpointMappings>
    <add Assembly="Divergent.Finance.Messages" Endpoint="Divergent.Finance" />
    <add Assembly="Divergent.Sales.Messages" Endpoint="Divergent.Sales" />
  </MessageEndpointMappings>
</UnicastBusConfig>
```
### Solution configuration

#### Start-up projects

* Divergent.Customers
* Divergent.Customers.API
* Divergent.Finance
* Divergent.Finance.API
* Divergent.Frontent
* Divergent.Sales
* Divergent.Sales.API
* Divergent.Shipping
* PaymentProviders
