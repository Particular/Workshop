# Exercise 2: Publish-Subscribe

**Important: Before attempting the exercise, please ensure you have followed [the instructions for preparing your machine](/README.md#preparing-your-machine-for-the-workshop) and that you have read [the instructions for running the exercise solutions](/README.md#running-the-exercise-solutions).**

NServiceBus endpoints communicate by sending each other messages. In this exercise, we'll focus on a specific type of messages: events. We will also explore a very powerful and popular messaging pattern: Publish-Subscribe (Pub-Sub).

Events are used to communicate that some action has taken place. They're informing us of a fact that something occurred in the past. In Pub-Sub, the sender (called Publisher) and the receiver (called Subscriber) are loosely coupled. There might be zero, one or multiple Subscribers interested in a specific event. In order to receive that event, they need to explicitly subscribe to it. In NServiceBus a subscription request is handled by the framework, together with message mappings and implementations of handlers which will process the event. The Publisher sends a copy of the event message to each subscriber.

## Overview

In the last exercise you extended the UI by showing additional information. For this exercise, the Orders page has a new button, "Create new order". In this exercise, we'll complete the process of placing a new order.

## Start-up projects

For more info, please see [the instructions for running the exercise solutions](/README.md#running-the-exercise-solutions).

* Divergent.CompositionGateway
* Divergent.Customers
* Divergent.Customers.API
* Divergent.Finance
* Divergent.Finance.API
* Divergent.Frontend
* Divergent.Sales
* Divergent.Sales.API
* Divergent.Shipping
* PaymentProviders

## Business requirements

When a customer creates a new order, that information is stored in a database. In order to complete the process, you need to provide the ability to pay for the placed order.

## What's provided for you

- Have a look at the `EndpointConfigurationExtensions` class in the `ITOps.SharedConfig` project. Note that we use conventions to specify which messages are events:

  ```c#
  conventions.DefiningEventsAs(t =>
      t.Namespace != null && t.Namespace.StartsWith("Divergent") && t.Namespace.EndsWith("Events") &&
      t.Name.EndsWith("Event"));
  ```
  
  If you create a class inside a namespace ending with "Events", and the name of this class ends with "Event", then NServiceBus will know it's an event.

- In the `Divergent.Finance/PaymentClient` directory you'll find a provided implementation for handling payments, named `ReliablePaymentClient`.


## Exercise 2.1: create and publish an `OrderSubmittedEvent`

In this exercise you'll create a new event named `OrderSubmittedEvent`. This event will be published by `SubmitOrderHandler` in the `Divergent.Sales` project.

### Step 1

Have a look at the following classes: `Customer` in `Divergent.Customers.Data.Models`, `Order` and `Product` in `Divergent.Sales.Data.Models`.

### Step 2

In the `Divergent.Sales.Messages` project, add a folder named `Events` and create an `OrderSubmittedEvent` class in the `Events` namespace. The class should have three properties with public setters and getters: the ID of the order, the ID of the customer, and the list of product IDs.

```c#
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

### Step 3

In `Divergent.Sales\Handlers\SubmitOrderHandler.cs`, at the end of the `Handle()` method, publish the `OrderSubmittedEvent` by calling `context.Publish()`. Copy the properties from the incoming `SubmitOrderCommand` message to the properties of the event.

```c#
await context.Publish(new OrderSubmittedEvent
{
    OrderId = order.Id,
    CustomerId = message.CustomerId,
    Products = message.Products,
});
```

## Exercise 2.2: handle `OrderSubmittedEvent` in Shipping, Finance and Customers

In this exercise, you'll subscribe to `OrderSubmittedEvent`, published by `Divergent.Sales`, by creating handlers named `OrderSubmittedHandler`, in the `Divergent.Shipping`, `Divergent.Finance`, and `Divergent.Customers` projects. You'll then extend the handler implementation in the `Divergent.Finance`, to process the payment using the provided `GetAmount()` method and the `ReliablePaymentClient` class.

### Step 1

In `Divergent.Shipping` project, add a folder named `Handlers` and create an `OrderSubmittedHandler` class inside the `Handlers` namespace.

### Step 2

The `OrderSubmittedHandler` should handle the `OrderSubmittedEvent` published by `Divergent.Sales`. To handle this event, implement the `IHandleMessages<OrderSubmittedEvent>` interface.

### Step 3

Use the provided logger to log information that the event was received and handled.

```c#
public class OrderSubmittedHandler : IHandleMessages<OrderSubmittedEvent>
{
    readonly ILogger<OrderSubmittedHandler> logger;

    public OrderSubmittedHandler(ILogger<OrderSubmittedHandler> logger)
    {
        this.logger = logger;
    }
    
    public async Task Handle(OrderSubmittedEvent message, IMessageHandlerContext context)
    {
        logger.LogDebug("Handle");

        // Store in database that order was submitted and which products belong to it.
        // Look at all pending orders, paid and ready to be shipped, in batches to decide what to ship.

        await Task.CompletedTask;
    }
}
```

### Step 4

In the `Divergent.Finance` project, add a folder named `Handlers` and create an `OrderSubmittedHandler` class inside the `Handlers` namespace.

### Step 5

The `OrderSubmittedHandler` should also process the `OrderSubmittedEvent` published by `Divergent.Sales`. To handle this event, implement the `IHandleMessages<OrderSubmittedEvent>` interface.

### Step 6

When Finance receives the `OrderSubmittedEvent` message it records the prices for the items that belong to the submitted order. This ensures that if product prices change, the customer will still be charged the amount which was shown to them in the UI (the value returned by `Divergent.Finance.API` at that time). Finally, Finance initiates the payment process by sending the `InitiatePaymentProcessCommand` message.

```c#
public class OrderSubmittedHandler : IHandleMessages<OrderSubmittedEvent>
{
    private readonly ILiteDbContext db;
    private readonly ILogger<OrderSubmittedHandler> logger;

    public OrderSubmittedHandler(ILiteDbContext db, ILogger<OrderSubmittedHandler> logger)
    {
        this.db = db;
        this.logger = logger;
    }

    public async Task Handle(OrderSubmittedEvent message, IMessageHandlerContext context)
    {
        logger.LogInformation("Handle OrderSubmittedEvent");

        double amount = 0;

        var prices = db.Database.GetCollection<Price>();
        var orderItemPrices = db.Database.GetCollection<OrderItemPrice>();
        
        var query = from price in prices.Query()
            where message.Products.Contains(price.ProductId)
            select price;

        foreach (var price in query.ToList())
        {
            var op = new OrderItemPrice
            {
                OrderId = message.OrderId,
                ItemPrice = price.ItemPrice,
                ProductId = price.ProductId
            };

            amount += price.ItemPrice;

            orderItemPrices.Insert(op);
        }

        await context.SendLocal(new InitiatePaymentProcessCommand
        {
            CustomerId = message.CustomerId,
            OrderId = message.OrderId,
            Amount = amount
        });
    }
}
```

### Step 7

In the `Divergent.Finance` project create the `InitiatePaymentProcessCommandHandler` class inside the `Handlers` namespace in order to handle the payment process.

```c#
class InitiatePaymentProcessCommandHandler : IHandleMessages<InitiatePaymentProcessCommand>
{
    private readonly ReliablePaymentClient reliablePaymentClient;
    private readonly ILogger<InitiatePaymentProcessCommandHandler> logger;

    public InitiatePaymentProcessCommandHandler(ReliablePaymentClient reliablePaymentClient, ILogger<InitiatePaymentProcessCommandHandler> logger)
    {
        this.reliablePaymentClient = reliablePaymentClient;
        this.logger = logger;
    }

    public async Task Handle(InitiatePaymentProcessCommand message, IMessageHandlerContext context)
    {
        logger.LogInformation("Handle InitiatePaymentProcessCommand");

        await reliablePaymentClient.ProcessPayment(message.CustomerId, message.Amount);
    }
}
```

### Step 8

In the `Divergent.Customers` project, add a folder named `Handlers` and create an  `OrderSubmittedHandler` class inside the `Handlers` namespace. This handler will keep track of the orders submitted by each customer.

```c#
public class OrderSubmittedHandler : NServiceBus.IHandleMessages<OrderSubmittedEvent>
{
    private readonly ILiteDbContext db;
    private readonly ILogger<OrderSubmittedHandler> log;

    public OrderSubmittedHandler(ILiteDbContext db, ILogger<OrderSubmittedHandler> log)
    {
        this.db = db;
        this.log = log;
    }

    public Task Handle(OrderSubmittedEvent message, NServiceBus.IMessageHandlerContext context)
    {
        log.LogInformation($"Handling: {nameof(OrderSubmittedEvent)}");

        var orders = db.Database.GetCollection<Order>();

        orders.Insert(new Order
        {
            CustomerId = message.CustomerId,
            OrderId = message.OrderId
        });
        
        return Task.CompletedTask;
    }
}
```

### Step 9

Run the solution. In the website, navigate to the "Orders" page and click the "Create new order" button. This should result in the `Divergent.Sales` endpoint publishing  `OrderSubmittedEvent`. The event will be received by `Divergent.Shipping`, `Divergent.Finance`, and `Divergent.Customers`. Verify this by navigating to the consoles and observing the log statements added in the handler classes.

## Exercise 2.3: create and publish the `PaymentSucceededEvent`

In this exercise we'll create a new event called `PaymentSucceededEvent`. This event will be published by the `InitiatePaymentProcessCommandHandler` in the `Divergent.Finance` project.

### Step 1

In the `Divergent.Finance.Messages` project, add a folder named `Events`, and create a class named `PaymentSucceededEvent.cs` in the `Events` namespace. The class should have a single property with a public setter and a getter: the ID of the order.

```c#
namespace Divergent.Finance.Messages.Events
{
    public class PaymentSucceededEvent
    {
        public int OrderId { get; set; }
    }
}
```

### Step 2

In `Divergent.Finance\Handlers\InitiatePaymentProcessCommandHandler.cs`, at the end of the `Handle()` method, publish the `PaymentSucceededEvent` by calling `context.Publish()`. Copy the order ID from the incoming `InitiatePaymentProcessCommand` message to the property of the event.

```c#
public async Task Handle(InitiatePaymentProcessCommand message, IMessageHandlerContext context)
{
    logger.LogInformation("Handle InitiatePaymentProcessCommand");

    await reliablePaymentClient.ProcessPayment(message.CustomerId, message.Amount);

    await context.Publish(new PaymentSucceededEvent
    {
        OrderId = message.OrderId,
    });
}
```

## Exercise 2.4: handle `PaymentSucceededEvent`

In this exercise we'll handle the `PaymentSucceededEvent` by logging the information in the `PaymentSucceededHandler` class in `Divergent.Shipping`.

### Step 1

Create the `PaymentSucceededHandler` class in the `Divergent.Shipping` project, in the `Handlers` namespace.

### Step 2

The `PaymentSucceededHandler` should process the `PaymentSucceededEvent` published by `Divergent.Finance`. In order to handle this event implement the `IHandleMessages<PaymentSucceededEvent>` interface in the `PaymentSucceededHandler` class.

### Step 3

Use the provided logger to log information that the event was received and handled.

```c#
public class PaymentSucceededHandler : IHandleMessages<PaymentSucceededEvent>
{
    readonly ILogger<PaymentSucceededHandler> logger;

    public PaymentSucceededHandler(ILogger<PaymentSucceededHandler> logger)
    {
        this.logger = logger;
    }

    public async Task Handle(PaymentSucceededEvent message, IMessageHandlerContext context)
    {
        logger.LogDebug("Handle");

        // Store in database that order was successfully paid.
        // Look at all pending orders, paid and ready to be shipped, in batches to decide what to ship.

        await Task.CompletedTask;
    }
}
```

### Step 4

Run the solution. In the website `Orders` page, click the "Create new order" button and verify that the `Divergent.Shipping` endpoint receives the `PaymentSucceededEvent`.

## Conclusion

This exercise has demonstrated how you can use the Publish-Subscribe messaging pattern to decouple services.

If you'd like to discuss this more, please don't hesitate to drop us a line in our [community discussion forum](https://discuss.particular.net/).
