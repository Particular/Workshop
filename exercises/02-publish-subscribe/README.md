# Exercise 2: Publish-Subscribe

**Important: Before attempting the exercise, please ensure you have followed [the instructions for preparing your machine](/README.md#preparing-your-machine-for-the-workshop) and that you have read [the instructions for running the exercise solutions](/README.md#running-the-exercise-solutions).**

NServiceBus endpoints communicate by sending each other messages. In this exercise, we'll focus on a specific type of messages: events. We will also explore a very powerful and popular messaging pattern: Publish-Subscribe (Pub-Sub).

Events are used to communicate that some action has taken place. They're informing us of a fact that something occurred in the past. In Pub-Sub, the sender (called Publisher) and the receiver (called Subscriber) are loosely coupled. There might be zero, one or multiple Subscribers interested in a specific event. In order to receive that event, they need to explicitly subscribe to it. In NServiceBus a subscription request is handled by the framework, together with message mappings and implementions of handlers which will process the event. The Publisher sends a copy of the event message to each subscriber.

## Overview

In the last exercise you extended the UI by showing additional information. For this exercise, the Orders page has a new button, "Create new order". In this exercise, we'll complete the process of placing a new order.

## Start-up projects

For more info, please see [the instructions for running the exercise solutions](/README.md#running-the-exercise-solutions).

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

- Have a look at the `EndpointConfig` class in the `Divergent.Finance` project. Note that we use conventions to specify which messages are events:

  ```c#
  conventions.DefiningEventsAs(t =>
      t.Namespace != null &&
      t.Namespace.StartsWith("Divergent") &&
      t.Namespace.EndsWith("Events") &&
      t.Name.EndsWith("Event"));`
  ```

  If you create a class inside a namespace ending with "Events", and the name of this class ends with "Event", then NServiceBus will know it's an event.

- In the `Divergent.Finance/PaymentClient` directory you'll find a provided implementation for handling payments, named `ReliablePaymentClient`.


## Exercise 2.1: create and publish an `OrderSubmittedEvent`

In this exercise you'll create a new event named `OrderSubmittedEvent`. This event will be published by `SubmitOrderHandler` in the `Divergent.Sales` project.

### Step 1

Have a look at the following classes: `Customer` in `Divergent.Customers.Data.Models`, `Order` and `Product` in `Divergent.Sales.Data.Models`.

### Step 2

In the `Divergent.Sales.Messages` project, in the directory `Events`, add a new class called `OrderSubmittedEvent.cs`. The class should have three properties with public setters and getters: Id of the order, Id of the customer, and the list of product Ids.

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

Have a look at `SubmitOrderHandler` class in the `Divergent.Sales` project. At the end of the `Handle` method, publish the `OrderSubmittedEvent`, by calling the ```context.Publish<OrderSubmittedEvent>()``` method. Copy the properties from the incoming `SubmitOrderCommand` message, to the properties of the event.

```c#
await context.Publish<OrderSubmittedEvent>(e =>
{
    e.OrderId = order.Id;
    e.CustomerId = message.CustomerId;
    e.Products = message.Products;
});
```

## Exercise 2.2: handle `OrderSubmittedEvent` in Shipping, Finance and Customers

In this exercise, you'll handle the `OrderSubmittedEvent` by logging the information in the `OrderSubmittedHandler` class in `Divergent.Shipping` project and in the `OrderSubmittedHandler` class in `Divergent.Sales` project. Then you'll extend the handler implementation in the `Divergent.Finance` project, in order to process the payment using the provided `GetAmount()` method and the `ReliablePaymentClient` class.

### Step 1

Create the `OrderSubmittedHandler` class in the `Divergent.Shipping` project, inside the `Handlers` namespace.

### Step 2

The `OrderSubmittedHandler` should process the `OrderSubmittedEvent` published by `Divergent.Sales`. In order to handle this event implement the `IHandleMessages<OrderSubmittedEvent>` interface in the `OrderSubmittedHandler` class.

### Step 3

In the `Divergent.Shipping` project navigate to `EndpointConfig` class and specify the publisher for the `OrderSubmittedEvent`. To do this use the `routing` object obtained when configuring the transport and add the following statement

```
routing.RegisterPublisher(typeof(OrderSubmittedEvent), "Divergent.Sales");
```

### Step 4

Use the provided logger to log information that the event was received and handled.

```c#
namespace Divergent.Shipping.Handlers
{
    public class OrderSubmittedHandler : IHandleMessages<OrderSubmittedEvent>
    {
        private static readonly ILog Log = LogManager.GetLogger<OrderSubmittedHandler>();

        public async Task Handle(OrderSubmittedEvent message, IMessageHandlerContext context)
        {
            Log.Info("Handle");
            await Task.CompletedTask;
        }
    }
}
```

### Step 5

Create an `OrderSubmittedHandler` class in the `Divergent.Finance` project, inside the `Handlers` namespace.

### Step 6

The `OrderSubmittedHandler` should also process the `OrderSubmittedEvent` published by `Divergent.Sales`. In order to handle this implement the `IHandleMessages<OrderSubmittedEvent>` interface in the `OrderSubmittedHandler` class.

### Step 7

In the `Divergent.Finance` project navigate to `EndpointConfig` class and specify the publisher for the `OrderSubmittedEvent`. To do this use the `routing` object obtained when configuring the transport and add the following statement

```
routing.RegisterPublisher(typeof(OrderSubmittedEvent), "Divergent.Sales");
```


### Step 8

When Finance receives the `OrderSubmittedEvent` message it records the prices for the items that belong to the submitted order. This ensures that if product prices change, the customer will still be charged the amount which was shown to them in the UI (the value returned by `Divergent.Finance.API` at that time). Finally, Finance initiates the payment process by sending the `InitiatePaymentProcessCommand` message.

```c#
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

### Step 9

In the `Divergent.Finance` project create the `InitiatePaymentProcessCommandHandler` class inside the `Handlers` namespace in order to handle the payment process.

```c#
namespace Divergent.Finance.Handlers
{
    public class InitiatePaymentProcessCommandHandler : IHandleMessages<InitiatePaymentProcessCommand>
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

### Step 10

In the `Divergent.Customers` project create the `OrderSubmittedHandler` class inside the `Handlers` namespace in order to keep track of which orders have been submitted by which customer.

```c#
namespace Divergent.Customers.Handlers
{
    public class OrderSubmittedHandler : IHandleMessages<OrderSubmittedEvent>
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

### Step 11

In the `Divergent.Customers` project, configure the publisher for the `OrderSubmittedEvent`. To do this use the `routing` object obtained when configuring the transport and add the following statement

```
routing.RegisterPublisher(typeof(OrderSubmittedEvent), "Divergent.Sales");
```

## Exercise 2.3: create and publish the `PaymentSucceededEvent`

In this exercise we'll create a new event called `PaymentSucceededEvent`. This event will be published by the `InitiatePaymentProcessCommandHandler` in the `Divergent.Finance` project.

### Step 1

In the `Divergent.Finance.Messages` project, in the directory `Events`, add a new class called `PaymentSucceededEvent.cs`. The class should have only a single property with a public setter and a getter: id of the order.

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

At the end of `InitiatePaymentProcessCommandHandler`, publish the `PaymentSucceededEvent` by calling `context.Publish<PaymentSucceededEvent>()` method. Copy the order id from the incoming `InitiatePaymentProcessCommand` message, to the property of the event.

```c#
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

## Exercise 2.4: handle `PaymentSucceededEvent`

In this exercise we'll handle the `PaymentSucceededEvent` by logging the information in the `PaymentSucceededHandler` class in `Divergent.Shipping`.

### Step 1

Create the `PaymentSucceededHandler` class in the `Divergent.Shipping` project, in the `Handlers` namespace.

### Step 2

The `PaymentSucceededHandler` should process the `PaymentSucceededEvent` published by `Divergent.Finance`. In order to handle this event implement the `IHandleMessages<PaymentSucceededEvent>` interface in the `PaymentSucceededEvent` class.

### Step 3

Use the provided logger to log information that the event was received and handled.

```c#
namespace Divergent.Shipping.Handlers
{
    public class PaymentSucceededHandler : IHandleMessages<PaymentSucceededEvent>
    {
        private static readonly ILog Log = LogManager.GetLogger<PaymentSucceededHandler>();

        public async Task Handle(PaymentSucceededEvent message, IMessageHandlerContext context)
        {
            Log.Info("Handle");
            await Task.CompletedTask;
        }
    }
}
```

### Step 4

In the `Divergent.Shipping` project, configure the publisher for the `PaymentSucceededEvent`. To do this use the `routing` object obtained when configuring the transport and add the following statement

```
routing.RegisterPublisher(typeof(PaymentSucceededEvent), "Divergent.Finance");
```


## Advanced exercise 2.5 : monitoring endpoints

**Important: Before attempting the advanced exercises, please ensure you have followed [the instructions for preparing your machine for the advanced exercises](README.md#preparing-your-machine-for-the-advanced-exercises).**

### Step 1

In Visual Studio, open the `Divergent.Customers` project and have a look at the endpoint configuration. There should be configuration for forwarding messages to the audit queue and sending poison messages to the error queue.

Our documentation contains more information about [auditing messages](https://docs.particular.net/nservicebus/operations/auditing) and how to configure [error handling](https://docs.particular.net/nservicebus/recoverability/configure-error-handling).

### Step 2

Verify that the audit and error queues have been created. You can use Windows Computer Management for this. Press <kbd>Win</kbd>+<kbd>X</kbd> to open the Windows system menu and select 'Computer Management'. You should see the queues Under "Service and Applications", "Message Queueing", "Private Queues".

NOTE: The MSMQ MMC snap-in is very limited in functionality. [QueueExplorer](http://www.cogin.com/mq/) is a great tool which provides much more.

### Step 3

After following the instructions for preparing your machine for the advanced exercises, an instance of ServiceControl should already be running as a Windows service. Run the exercise solution again and perform some actions to ensure that it has processed some messages.

### Step 4

Now, Let's have a look at ServicePulse.

Navigate to http://localhost:9090/

The list of 'Last 10 events' shows that messages have been received from at least one endpoint, but those endpoints are not yet being monitored. To do that, we need to install the monitoring plugin into those endpoints.

Note: If ServicePulse doesn't seem to be running, or it cannot connect to ServiceControl, verify that both instances are running as Windows services. By default, the names of both services begin with "Particular".

### Step 5

Let's install the ServiceControl [heartbeat plugin]((https://docs.particular.net/servicecontrol/plugins/heartbeat)) into the NServiceBus endpoints. Install this plugin into every project that hosts an endpoint via the Visual Studio NuGet user interface or via the Package Manager Console.

### Step 6

The heartbeat plugin sends messages directly to the ServiceControl queue rather than using the audit or error queues. The documentation shows how to configure the endpoint and tell it which queue it should send heartbeat messages to.

You can find out the name of the queue by accessing the 'ServiceControl Management' app in the Windows Start menu. The name of the instance is also the name of the queue.

Make sure you configure every project that hosts an endpoint. You can easily copy & paste this since every project should be sending heartbeats to the same queue.

### Step 7

Run the solution and naviate to ServicePulse while the projects are starting.

After a while the 'Last 10 events' should show that some of the `Divergent` endpoints have started. Soon afer it will show that those endpoints are running the heartbeats plugin.

### Step 8

Turn off the endpoints by stopping debugging in Visual Studio or shutting down the console windows.

ServiceControl is expecting heartbeat messages from every endpoint. If it doesn't continue to receive them, it will wait a little while (the default is 40 seconds) and then report that those endpoints are down.

When you restart the projects, ServicePulse should report that the endpoints are up again.

### Step 9

At the top of the page in ServicePulse, you will see a menu with various options. Check 'Failed Messages' to see if there are any messages that failed to be processed.

If you see nothing there, try and simluate a failure by adding the throwing of an exception to a message handler and running the solution with that in place. Due to immediate and delayed retries it might take a while for the message to end up in the error queue. You can read up on how to configure [immediate](https://docs.particular.net/nservicebus/recoverability/configure-immediate-retries) and [delayed](https://docs.particular.net/nservicebus/recoverability/configure-delayed-retries) retries so that they will be either sped-up or disabled.

See how the messages can be group and retried individually or per group.

Next, stop the solution, remove the throwing of the exception and restart it. Try retrying the failed message in ServicePulse and see that it should now be processed successfully.

This is a powerful feature which could be of huge value to operations activities. Imagine a system with high throughput. To perform maintenance, the database containing your business data was brought offline for a couple of minutes. Tthousands of messages ended up in the error queue and you can see those in ServicePulse. Once the system is up and running again, we can retry them and they should be processed successfully.

### Step 10

We now have a dashboard that can inform us when an endpoint is  or messages failed to be processed. A few things to consider:

- No-one wants to watch the dashboard all day. Fortunately, ServiceControl also uses pub/sub to notify subscribers of events. You can build an endpoint that subscribes to ServiceControl events and informs you of downtime or failed email, SMS or other means. Read more about [using ServiceControl events](https://docs.particular.net/servicecontrol/contracts).
- You might notice several endpoints with the same name. Endpoints send heartbeats with a unique host identifier, made up of their endpoint name and a hash of the folder the endpoint is installed in. Our exercises all have the same endpoint name, but different folders. Another example is when you deploy endpoints using [Octopus](https://octopus.com/). This will deploy every version in its own folder, with the result that every version will spawn a new monitored endpoint in ServicePulse. You can solve this by [overriding the host identifier](https://docs.particular.net/nservicebus/hosting/override-hostid).

## Conclusion

This exercise has demonstrated how you can use the Publish-Subscribe messaging pattern to decouple services.

If you'd like to discuss this more, please don't hesitate to drop us a line in our [community discussion forum](https://discuss.particular.net/).
