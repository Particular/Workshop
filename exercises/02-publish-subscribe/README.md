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

- Have a look at the `EndpointConfigurationExtensions` class in the `ITOps.EndpointConfig` project. Note that we use conventions to specify which messages are events:

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

In the `Divergent.Sales.Messages` project, create a new folder for `Events` and then add a new class called `OrderSubmittedEvent.cs`. The class should have three properties with public setters and getters: Id of the order, Id of the customer, and the list of product Ids.

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

In the `Divergent.Sales` project, have a look at `Handlers\SubmitOrderHandler.cs` class. At the end of the `Handle` method, publish the `OrderSubmittedEvent`, by calling the ```context.Publish()``` method. Copy the properties from the incoming `SubmitOrderCommand` message, to the properties of the event.

```c#
await context.Publish(new OrderSubmittedEvent
{
    OrderId = order.Id,
    CustomerId = message.CustomerId,
    Products = message.Products,
});
```

## Exercise 2.2: handle `OrderSubmittedEvent` in Shipping, Finance and Customers

In this exercise, you'll subscribe to the `OrderSubmittedEvent` published by `Divergent.Sales` by creating new handlers, named `OrderSubmittedHandler`, in both the `Divergent.Shipping` and the `Divergent.Sales` projects. Then you'll extend the handler implementation in the `Divergent.Finance` project, in order to process the payment using the provided `GetAmount()` method and the `ReliablePaymentClient` class.

### Step 1

In `Divergent.Shipping` project, add a new folder for `Handlers` and then create a new `OrderSubmittedHandler` class inside the `Handlers` namespace.

### Step 2

The `OrderSubmittedHandler` should process the `OrderSubmittedEvent` published by `Divergent.Sales`. In order to handle this event implement the `IHandleMessages<OrderSubmittedEvent>` interface in the `OrderSubmittedHandler` class.

### Step 3

In the `Divergent.Shipping` project navigate to `Host` class and specify the publisher for the `OrderSubmittedEvent`. To do this in the `Start` method, use the `routing` object obtained when configuring the transport and add the following statement:

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

In the `Divergent.Finance` project, add a new folder for `Handlers` and then create a new `OrderSubmittedHandler` class inside the `Handlers` namespace.

### Step 6

The `OrderSubmittedHandler` should also process the `OrderSubmittedEvent` published by `Divergent.Sales`. In order to handle this implement the `IHandleMessages<OrderSubmittedEvent>` interface in the `OrderSubmittedHandler` class.

### Step 7

In the `Divergent.Finance` project navigate to `Host` class and specify the publisher for the `OrderSubmittedEvent`. To do this in the `Start` method, use the `routing` object obtained when configuring the transport and add the following statement:

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

In the `Divergent.Customers` project, add a new folder for `Handlers` and then create a new `OrderSubmittedHandler` class inside the `Handlers` namespace in order to keep track of which orders have been submitted by which customer.

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

In the `Divergent.Customers` project navigate to `Host` class and specify the publisher for the `OrderSubmittedEvent`. To do this, in the `Start` method, use the `routing` object obtained when configuring the transport and add the following statement

```
routing.RegisterPublisher(typeof(OrderSubmittedEvent), "Divergent.Sales");
```

### Step 12

Run the solution. In the website, navigate to the `Orders` page and click the `Create new order` button. This should trigger the `Divergent.Sales` endpoint to publish the `OrderSubmittedEvent`. This event will now be received by  `Divergent.Customers`, `Divergent.Finance` and `Divergent.Shipping`. Verify this by navigating to the console and verifying the log statements added in the handler class.

## Exercise 2.3: create and publish the `PaymentSucceededEvent`

In this exercise we'll create a new event called `PaymentSucceededEvent`. This event will be published by the `InitiatePaymentProcessCommandHandler` in the `Divergent.Finance` project.

### Step 1

In the `Divergent.Finance.Messages` project, add a new folder called `Events`, and then create a new class called `PaymentSucceededEvent.cs` in the `Events` namespace. The class should have only a single property with a public setter and a getter: id of the order.

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

In the `Divergent.Finance` project, locate the `InitiatePaymentProcessCommandHandler` class. In the `Handle` method, as the last step, publish the `PaymentSucceededEvent` by calling `context.Publish()` method. Copy the order id from the incoming `InitiatePaymentProcessCommand` message, to the property of the event.

```c#
public async Task Handle(InitiatePaymentProcessCommand message, IMessageHandlerContext context)
{
    Log.Info("Handle InitiatePaymentProcessCommand");

    await _reliablePaymentClient.ProcessPayment(message.CustomerId, message.Amount);

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

In the `Divergent.Shipping` project, configure the publisher for the `PaymentSucceededEvent`. To do this locate `Host.cs` and in the `Start` method, use the `routing` object obtained when configuring the transport and add the following statement

```
routing.RegisterPublisher(typeof(PaymentSucceededEvent), "Divergent.Finance");
```

### Step 5

Run the solution. In the website, `Orders` page, click the `Create new order` button and verify that the `Divergent.Shipping` endpoint is now also receiving the `PaymentSucceededEvent`.

## Advanced exercise 2.5 : monitoring endpoints

**Important: Before attempting the advanced exercises, please ensure you have followed [the instructions for preparing your machine for the advanced exercises](/README.md#preparing-your-machine-for-the-advanced-exercises).** Specifically:

- [Configure ServiceControl Instance](/README.md#configure-servicecontrol-instance)
- [Configure Monitoring Instance](/README.md#configure-monitoring-instance)

### Step 1

In Visual Studio, open the `Divergent.Customers` project and have a look at the endpoint configuration. There should be configuration for forwarding messages to the audit queue and sending poison messages to the error queue.

Our documentation contains more information about [auditing messages](https://docs.particular.net/nservicebus/operations/auditing) and how to configure [error handling](https://docs.particular.net/nservicebus/recoverability/configure-error-handling).

### Step 2

Verify that the audit and error queues have been created. You can use Windows Computer Management for this. Press <kbd>Win</kbd>+<kbd>X</kbd> to open the Windows system menu and select 'Computer Management'. You should see the queues Under "Service and Applications", "Message Queueing", "Private Queues".

NOTE: The MSMQ MMC snap-in is very limited in functionality. [QueueExplorer](http://www.cogin.com/mq/) is a great tool which provides much more.

### Step 3

After following the instructions for preparing your machine for the advanced exercises, an instance of ServiceControl should already be running as a Windows service. Run the exercise solution again and `create a new order` from the `Orders` page just to ensure that it has processed some messages.

### Step 4

Now, Let's have a look at ServicePulse.

Navigate to http://localhost:9090/

You'll see an empty dashboard. From the menu, click the `Configuration`. The `Endpoint Heartbeats` section will show the list of endpoints that we have been using. However these endpoints are not yet being monitored. To do that, we need to install the monitoring plugin into those endpoints.

Note: If ServicePulse doesn't seem to be running, or it cannot connect to ServiceControl, verify that both instances are running as Windows services. By default, the names of both services begin with "Particular".

### Step 5

Let's install the ServiceControl [heartbeat plugin](https://docs.particular.net/monitoring/heartbeats/install-plugin?version=heartbeats_3) into the NServiceBus endpoints.

Install this plugin in the ITOps.EndpointConfig project which will be referenced by endpoints. You can do this via the Visual Studio NuGet user interface or via the Package Manager Console.

To install the plugin type: `Install-Package NServiceBus.Heartbeat -Version 3.0.0 -Project ITOps.EndpointConfig` in Package Manager Console.

If you use `Manage Nuget Packages` option, make sure you select **version 3.0.0**


### Step 6

The heartbeat plugin sends messages directly to the ServiceControl queue rather than using the audit or error queues. The documentation shows how to configure the endpoint and tell it which queue it should send heartbeat messages to.

You can find out the name of the queue by accessing the 'ServiceControl Management' app in the Windows Start menu. The name of the instance is also the name of the queue.

In the `ITOps.EndpointConfig` project, in `EndpointCongigurationExtensions.cs`, in the `Configure` method add the following configuration to enable heartbeats:

```
endpointConfiguration.SendHeartbeatTo(
  serviceControlQueue: "Particular.ServiceControl",
  frequency: TimeSpan.FromSeconds(15),
  timeToLive: TimeSpan.FromSeconds(30));
```

### Step 7

Add the ServiceControl metrics component so that important metrics like message throughput, etc can be monitored and viewed in the ServicePulse dashboard.

Let's install the ServiceControl [Metrics component](https://docs.particular.net/monitoring/metrics/install-plugin) into the NServiceBus endpoints.

Install this plugin in the ITOps.EndpointConfig project which will be referenced by endpoints. You can do this via the Visual Studio NuGet user interface or via the Package Manager Console.

To install the plugin type: `Install-Package NServiceBus.Metrics.ServiceControl -Project ITOps.EndpointConfig` in Package Manager Console.

In the `ITOps.EndpointConfig` project, in `EndpointCongigurationExtensions.cs`, in the `Configure` method add the following configuration to enable reporting metrics to ServicePulse:

```
var metrics = endpointConfiguration.EnableMetrics();
metrics.SendMetricDataToServiceControl(
  serviceControlMetricsAddress: "particular.monitoring",
  interval: TimeSpan.FromSeconds(10));
```


### Step 8

Run the solution and navigate to ServicePulse while the projects are starting.

Make sure to create a few orders in the Orders page in the front end application.

In the ServicePulse menu, navigate to the `Monitoring` option and review the various metrics.

### Step 9

Turn off the endpoints by stopping debugging in Visual Studio or shutting down the console windows.

ServiceControl is expecting heartbeat messages from every endpoint. If it doesn't continue to receive them, it will wait a little while (30 seconds) and then report that those endpoints are down.

When you restart the projects, ServicePulse should report that the endpoints are up again.

### Step 10

At the top of the page in ServicePulse, you will see a menu with various options. Check 'Failed Messages' to see if there are any messages that failed to be processed.

If you see nothing there, try and simulate a failure by adding the throwing of an exception to a message handler and running the solution with that in place. Due to immediate and delayed retries it might take a while for the message to end up in the error queue. You can read up on how to configure [immediate](https://docs.particular.net/nservicebus/recoverability/configure-immediate-retries) and [delayed](https://docs.particular.net/nservicebus/recoverability/configure-delayed-retries) retries so that they will be either sped-up or disabled.

See how the messages can be group and retried individually or per group.

Next, stop the solution, remove the throwing of the exception and restart it. Try retrying the failed message in ServicePulse and see that it should now be processed successfully.

This is a powerful feature which could be of huge value to operations activities. Imagine a system with high throughput. To perform maintenance, the database containing your business data was brought offline for a couple of minutes. Thousands of messages ended up in the error queue and you can see those in ServicePulse. Once the system is up and running again, we can retry them and they should be processed successfully.

### Step 11

We now have a dashboard that can inform us when an endpoint is or messages failed to be processed. A few things to consider:

- No-one wants to watch the dashboard all day. Fortunately, ServiceControl also uses pub/sub to notify subscribers of events. You can build an endpoint that subscribes to ServiceControl events and informs you of downtime or failed email, SMS or other means. Read more about [using ServiceControl events](https://docs.particular.net/servicecontrol/contracts).
- You might notice several endpoints with the same name. Endpoints send heartbeats with a unique host identifier, made up of their endpoint name and a hash of the folder the endpoint is installed in. Our exercises all have the same endpoint name, but different folders. Another example is when you deploy endpoints using [Octopus](https://octopus.com/). This will deploy every version in its own folder, with the result that every version will spawn a new monitored endpoint in ServicePulse. You can solve this by [overriding the host identifier](https://docs.particular.net/nservicebus/hosting/override-hostid).

## Conclusion

This exercise has demonstrated how you can use the Publish-Subscribe messaging pattern to decouple services.

If you'd like to discuss this more, please don't hesitate to drop us a line in our [community discussion forum](https://discuss.particular.net/).
