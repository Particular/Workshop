# Exercise 02 - Publish-Subscribe

In NServiceBus endpoints communicate by sending each other messages. In this exercise we'll focus on special kind of messages - events. We will also explore a very powerful and popular messaging pattern - Publish-Subscribe (often also called Pub-Sub).

Events are used to communicate that some action has taken place, they're informing about a fact that occurred in the past. In Pub-Sub the sender (called Publisher) and the receiver (called Subscriber) are loosely coupled. There might be zero, one or multiple Subscribers interested in a specific event. In order to receive that event they need to explicitly subscribed to it. In NServiceBus the subscription request is sent by the framework, after specifying mappings and implementing a handler which will process the event. The Publisher sends a copy of the event message to every subscriber.

## Introduction

In the last exercise you've extended the UI by showing additional information. As you probably noticed, in the Orders page we have a button "Create new order". In this exercise we'll complete the process of placing a new order.

### Business requirement

At the moment when a customer creates a new order that information is just saved in a database. In order to complete the process, you need to provide the ability to pay for the placed order.

### What's provided for you:
- Have a look at the `EndpointConfig` class in the `Divergent.Finance` project. Note that we use conventions to specify which messages are events:

```conventions.DefiningEventsAs(t => t.Namespace != null && t.Namespace.StartsWith("Divergent") && t.Namespace.EndsWith("Events") && t.Name.EndsWith("Event"));```

If you create a class inside the namespace which name ends with "Events" and the name of this class will end with "Event", then NServiceBus will know it's an event.

- In the `Divergent.Finance` project inside `PaymentClient` directory you can find a provided implementation for handling payments called `ReliablePaymentClient`.


## Exercise 02.1 - Create and publish the `OrderSubmittedEvent`

In this exercise you'll create a new event called `OrderSubmittedEvent`. That event will be published by `SubmitOrderHandler` in the `Divergent.Sales` project.

**1)** Compile the application to retrieve all NuGet packages.

**2)** Have a look at the following classes: `Customer` in the `Divergent.Customers.Data.Models`, `Order` and `Product` in the `Divergent.Sales.Data.Models`.

**3)** In the `Divergent.Sales.Messages` project, in the directory `Events` add a new class called `OrderSubmittedEvent.cs`. The class should have three properties with public setters and getters: id of the order, id of the customer and the list of product ids.

```
namespace Divergent.Sales.Messages.Events
{
    public class OrderSubmittedEvent
    {
        public Guid OrderId { get; set; }
        public Guid CustomerId { get; set; }

        public List<Guid> Products { get; set; }
    }
}
```

**4)** Have a look at `SubmitOrderHandler` class in the `Divergent.Sales` project. At the end of the handler publish the `OrderSubmittedEvent` by calling ```context.Publish<OrderSubmittedEvent>()``` method. Copy the properties from the incoming `SubmitOrderCommand` message, to the properties of the event.

```
await context.Publish<OrderSubmittedEvent>(e =>
  {
      e.OrderId = message.OrderId;
      e.CustomerId = message.CustomerId;
      e.Products = message.Products;
  });
```

## Exercise 02.2 - Handle the `OrderSubmittedEvent`

In this exercise you'll handle the `OrderSubmittedEvent` by logging the information in the `OrderSubmittedHandler` class in `Divergent.Shipping` project and in the `OrderSubmittedHandler` class in `Divergent.Sales` project. Then we'll extend handler implementation in the `Divergent.Finance` project in order to process the payment by using provided `GetAmount()` method and `ReliablePaymentClient` class.

**1)** Have a look at the `OrderSubmittedHandler` class in the `Divergent.Shipping` project.

**2)** The `OrderSubmittedHandler` should process the `OrderSubmittedEvent` published by `Divergent.Sales`. In order to handle it implement the `IHandleMessages<OrderSubmittedEvent>` interface in the `OrderSubmittedHandler` class.

**3)** In the `App.config` file in the `Divergent.Shipping` project add a new configuration section called `UnicastBusConfig` and specify `MessageEndpointMappings` for the assembly containing `OrderSubmittedEvent` event. The information provided in the mapping is used by NServiceBus internally to send subscription request from the subscriber to the publisher.

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

**5)** Have a look at the `OrderSubmittedHandler` class in the `Divergent.Finance` project.

**6)** The `OrderSubmittedHandler` should also process the `OrderSubmittedEvent` published by `Divergent.Sales`. In order to handle it implement the `IHandleMessages<OrderSubmittedEvent>` interface in the `OrderSubmittedHandler` class.

**7)** In the `App.config` file in the `Divergent.Finance` project add a new configuration section called `UnicastBusConfig` and specify `MessageEndpointMappings` for the assembly containing `OrderSubmittedEvent` event.

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

**8)** Use the provided logger to log information that the event was received and handled.

```
namespace Divergent.Finance.Handlers
{
    public class OrderSubmittedHandler : IHandleMessages<OrderSubmittedEvent>
    {
        public async Task Handle(OrderSubmittedEvent message, IMessageHandlerContext context)
        {
            Log.Info("Handle OrderSubmittedEvent");            
        }
    }
}
```

**9)**  In the `Divergent.Finance` project go back to the `Handle` method in the `OrderSubmittedHandler`. Extend the method by using provided `GetAmount` method and `_reliablePaymentClient` to process the payment.

```
public async Task Handle(OrderSubmittedEvent message, IMessageHandlerContext context)
{
  Log.Info("Handle OrderSubmittedEvent");

  var amount = await GetAmount(message.Products);
  await _reliablePaymentClient.ProcessPayment(message.CustomerId, amount);
}
```

## Exercise 02.3 - Create and publish the `PaymentSucceededEvent`

In this exercise we'll create a new event called `PaymentSucceededEvent`. That event will be published by `OrderSubmittedHandler` in the `Divergent.Finance` project.

**1)** In the `Divergent.Finance.Messages` project, in the directory `Events` add a new class called `PaymentSucceededEvent.cs`. The class should have only a single property with public setters and getters: id of the order.

```
namespace Divergent.Finance.Messages.Events
{
    public class PaymentSucceededEvent
    {
        public Guid OrderId { get; set; }
    }
}
```

**2)**  At the end of the handler publish the `PaymentSucceededEvent` by calling ```context.Publish<PaymentSucceededEvent>()``` method. Copy the order id from the incoming `OrderSubmittedEvent` message, to the property of the event.

```
public async Task Handle(OrderSubmittedEvent message, IMessageHandlerContext context)
{
  Log.Info("Handle OrderSubmittedEvent");

  var amount = await GetAmount(message.Products);
  await _reliablePaymentClient.ProcessPayment(message.CustomerId, amount);

  await context.Publish<PaymentSucceededEvent>(e =>
    {
       e.OrderId = message.OrderId;
    });
}
```

## Exercise 02.4 - Handle the `PaymentSucceededEvent`

In this exercise we'll handle the `PaymentSucceededEvent` by logging the information in the `PaymentSucceededHandler` class in `Divergent.Shipping`.

**1)** Have a look at the `PaymentSucceededHandler` class in the `Divergent.Shipping` project.

**2)** The `PaymentSucceededHandler` should process the `PaymentSucceededEvent` published by `Divergent.Finance`. In order to handle it implement the `IHandleMessages<PaymentSucceededEvent>` interface in the `PaymentSucceededEvent` class.

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
