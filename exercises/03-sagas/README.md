# Exercise 3: Sagas

**Important: Before attempting the exercise, please ensure you have followed [the instructions for preparing your machine](/README.md#preparing-your-machine-for-the-workshop) and that you have read [the instructions for running the exercise solutions](/README.md#running-the-exercise-solutions).**

In NServiceBus a saga is the coordinator of a long running (business) process. They are very powerful and once fully grasped, can greatly increase flexibility in projects.

## Overview

In the last exercise you published events in the `Sales` and `Finance` verticals. The `Shipping`, `Finance` and `Customers` vertical have components that are subscribed to those events. When event arrives, state is stored or updated in the database at each service.

The following events are currently published in the system:
- `OrderSubmittedEvent` - The ordered products are attached to this event, through which we're able to ship them.
- `PaymentSucceededEvent` - The order was successfully paid and we can ship it.

NOTE: In order to view saga details in ServiceInsight, the [SagaAudit Plugin](https://docs.particular.net/nservicebus/sagas/saga-audit?version=sagaaudit_3) must be installed in the endpoints which contain the sagas. Currently, the solution does not have this plugin installed so you will have to install it yourself before running the endpoints, if you wish to view the details of your sagas in ServiceInsight.

To install the plugin type: `Install-Package NServiceBus.SagaAudit -Version 3.0.0`, in Package Manager Console.

If you use `Manage Nuget Packages` option, make sure you select **version 3.0.0**

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

At the moment we gather all successfully paid orders and the only option we have is to process them in batches. Business is growing rapidly and we would like to process orders as soon as possible, not just at predefined intervals.

Note: This is a hypothetical case. From experience we've seen a lot of developers implement batch processes. Sagas are a better alternative to batch processes. For more info, read our blog post, "[Death to the batch job](https://particular.net/blog/death-to-the-batch-job)".

## Issues

- After the handlers for both events store data in the database, we need a batch job to gather all successfully paid orders. It is impossible to run this batch job continuously throughout the day, as this will largely increase database locks and interfere with other operations happening in the system. We want to process orders near real-time.

- The events can arrive out-of-order, because the database could be locked or in maintenance mode and the `OrderSubmittedEvent` was set aside to be processed later by Delayed Retries. In the meantime the `PaymentSucceededEvent` might arrive. We need to make sure the order in which we receive these events doesn't matter.

- When more events or actions are added to this business process, the implementation gets scattered across various handlers and classes. We want high cohesion and make sure the implementation makes as much sense as possible and is easy to debug and manage.

## Exercise 3.1: replace the handlers with a saga

In this exercise you'll replace the two handlers in `Shipping` with a saga. The single saga will handle two events.

### Step 1

Have a look at the `OrderSubmittedHandler` and `PaymentSucceededHandler` in `Divergent.Shipping` project.

These are the handlers receiving the accompanying events. The events that can arrive out of order.

Remove both handlers.

### Step 2

We're now left with an empty `Handlers` folder. **Rename** it to `Sagas`.

### Step 3

Add a new class to this folder called `ShippingSaga.cs` and have it implement the `Saga<T>` class. We'll start out with `object` for T and get back later to what it does. You should end up with your class looking like this:

```c#
class ShippingSaga : Saga<object>
{
}
```

### Step 4

Now we will have our saga implement both handlers for `OrderSubmittedEvent` and `PaymentSucceededEvent`. Have the class implement the `IAmStartedByMessages<T>` interface for both events.

NOTE: We're not using `IHandleMessages<T>` to initiate the saga, but rather `IAmStartedByMessages<T>`. The reason is that some messages can start the saga, whereas others should assume the saga to be already instantiated. At least one message should start the saga. We'll get back to this later with more details.

The class should now look like this:

```c#
class ShippingSaga : Saga<object>,
    IAmStartedByMessages<OrderSubmittedEvent>,
    IAmStartedByMessages<PaymentSucceededEvent>
{
}
```

Override saga base class `ConfigureHowToFindSaga` abstract method and implement both interfaces, leaving methods bodies as is. Before we can implement these, we need to add some saga state.

## Exercise 3.2: add state to the saga

A saga is coordinating long-running business processes. The saga you implement, takes decisions based on data that it receives. Imagine a business process that is running for days, years even. Some business processes actually never end. Our saga can't keep all that data in memory forever, so it needs to store this data somewhere.

We need to define what state we want to store for our saga. This saga is about a customer's order, so we should store both the `CustomerId` and the `OrderId`. We also want to store the products coming in with the `OrderSubmittedEvent`.

### Step 1

Define a new class called `ShippingSagaData` and have it inherit from `ContainSagaData`. Add two properties of type `int` for CustomerId and OrderId. You should have a class like this.

```c#
class ShippingSagaData : ContainSagaData
{
    public int OrderId { get; set; }
    public int CustomerId { get; set; }
}
```

### Step 2

Now we'll add products. A minor problem is that we can't add `IList<int>` for the products as NServiceBus persisters can't map this properly to tables or documents in SQL Server and/or RavenDB. We need a complex type. Create a class called `Product`, and in `ShippingSagaData` add a property of `ICollection<Product>` to contain the products. Of course the `Product` class needs to hold the unique id of each ordered product, so we need to add a property for that as well. We'll end up with a class like this:

```c#
class ShippingSagaData : ContainSagaData
{
    public int OrderId { get; set; }
    public int CustomerId { get; set; }
    public ICollection<Product> Products { get; set; }

    public class Product
    {
        public int Identifier { get; set; }
    }
}
```

### Step 3

Now we'll add this class as saga state to our saga. We'll end up with a class like this:

```c#
class ShippingSaga : Saga<ShippingSagaData>,
    IAmStartedByMessages<OrderSubmittedEvent>,
    IAmStartedByMessages<PaymentSucceededEvent>
{
}
```

### Step 4

Make sure everything compiles.

### Step 5

Store the `CustomerId` and products in the state of the saga. We can do this by accessing the saga's `Data` property, as in `Data.CustomerId`. Set the appropriate properties in both `Handle` methods. Note that we _do not_ set `Data.OrderId` (see next exercise).

```c#
public Task Handle(OrderSubmittedEvent message, IMessageHandlerContext context)
{
    Data.CustomerId = message.CustomerId;

    var projection = message.Products.Select(p => new ShippingSagaData.Product { Identifier = p });
    Data.Products = projection.ToList();
    return Task.CompletedTask;
}

public Task Handle(PaymentSucceededEvent message, IMessageHandlerContext context)
{
    return Task.CompletedTask;
}
```

## Exercise 3.3: map incoming messages to the saga

In this exercise you will map incoming messages to the saga so that NServiceBus knows which property to use to find and retrieve the correct saga instance from the saga data storage.

### Step 1

Open the `ShippingSaga` class and locate the `ConfigureHowToFindSaga` method.

### Step 2

Map the `OrderSubmittedEvent` property `OrderId` to the sagas `OrderId` property. Do this by overriding The `ConfigureHowToFindSaga` method on the saga. It provides a `mapper` object as an argument. The mapper object exposes a `ConfigureMapping` method, which takes the event type and property to match, as well as the saga property you want to map it to. Map the `PaymentSucceededEvent` property `OrderId` to the sagas `OrderId` property.:

```c#
mapper.ConfigureMapping<OrderSubmittedEvent>(p => p.OrderId).ToSaga(s => s.OrderId);
mapper.ConfigureMapping<PaymentSucceededEvent>(p => p.OrderId).ToSaga(s => s.OrderId);
```

Note that this mapping also tells NServiceBus how to set the value of `Data.OrderId`. This is why we did not have to set `Data.OrderId` ourselves in exercise 2.2.

## Exercise 3.4 - deal with out-of-order delivery

In this exercise you will process the messages coming in and make sure the messages can arrive in any order, by verifying if all expected messages have been received.

### Step 1

There are various ways to check in what state the saga is. We can add flags to the saga state to verify which steps already happened.

### Step 2

To verify if `PaymentSucceededEvent` has been received, we can set a boolean property on the saga state. Add a boolean property to `ShippingSagaData` called `IsPaymentProcessed` and in the handler for the `PaymentSuccedeedEvent` we set this property to true.

```c#
public async Task Handle(PaymentSucceededEvent message, IMessageHandlerContext context)
{
    Data.IsPaymentProcessed = true;
    return Task.CompletedTask;
}
```

NOTE: If the `IsPaymentProcessed` property is generated using refactoring tools, it may have a internal setter:

```c#
public bool IsPaymentProcessed { get; internal set; }
```

Make sure you remove the `internal` keyword to make the setter public. Otherwise, when `ShippingSagaData` is hydrated from storage, the property may not be set correctly.

### Step 3

To verify if `OrderSubmittedEvent` has been received, we can set a boolean property on the saga state. Add a boolean property to `ShippingSagaData` called `IsOrderSubmitted`, and in the handler for the `OrderSubmittedEvent` set this property to true.

NOTE: If the `IsOrderSubmitted` property is generated using refactoring tools, it may have a internal setter:

```c#
public bool IsOrderSubmitted { get; internal set; }
```

Make sure you remove the `internal` keyword to make the setter public. Otherwise, when `ShippingSagaData` is hydrated from storage, the property may not be set correctly.

The client or service should verify if its a valid order and not have the user be able to submit the order without any products. Commands and events can technically fail, but should not functionally fail because proper validation was not done on the sending side.

### Step 4

In the saga add a new async method called `ProcessOrder` that you will call from both `Handle` methods. Inside this method, verify if the saga state property `IsPaymentProcessed` was set and if the `IsOrderSubmitted` property was set as well. If yes, invoke `MarkAsComplete()`. This method signals NServiceBus that we're done with this saga and that it can be removed from the underlying storage.

```c#
    private async Task ProcessOrder(IMessageHandlerContext context)
    {
        if (Data.IsOrderSubmitted && Data.IsPaymentProcessed)
        {
            await Task.CompletedTask; // Send a message to execute shipment
            MarkAsComplete();
        }
    }
```


## Advanced Exercise 3.5

**Important: Before attempting the advanced exercises, please ensure you have followed [the instructions for preparing your machine for the advanced exercises](/README.md#preparing-your-machine-for-the-advanced-exercises).**

Sagas are excellent for coordinating a business process. In the current saga we're only able to execute the happy path of the business process, where everything works. Payment succeeds and acknowledgement of this arrives at our Shipping service within a short time. But what if the `PaymentSucceededEvent` never arrives? Our Finance service is the authority which should decide when a payment takes too long, but it has no knowledge of how to contact customers. The Customers service however, would probably have details of how every customer would like to be contacted. This is just an example, but you can probably get an idea of how important it is to properly define your boundaries with real projects.

The `InitiatePaymentProcessCommand`, rather than being processed by a stateless handler, should start a saga to orchestrate this long running process. This saga can then initiate the actual payment, and at the same time send a timeout message to itself. If the timeout message arrives back at the saga before the acknowledgement of successful payment, we can publish an event that the Customer service can react to.

NOTE: The `InitiatePaymentProcessCommandHandler` currently calls `ReliablePaymentClient`, but sagas should only orchestrate a business process and never perform actions themselves. So instead of moving all the code from `InitiatePaymentProcessCommandHandler` into the saga, you should create another message and send that to this handler. You should rename this handler appropriately.

To complete this exercise, take the following steps:

- Create a new saga to orchestrate the business logic which should be performed when payments do not succeed in an acceptable time.
- Make sure the saga only orchestrates the process by sending/publishing appropriate messages, and doesn't perform any actions itself.
- Subscribe to the appropriate event in the Customers service, and contact the customer when required.

Bear in mind that in production, a small business requirement like this could spawn many more messages. Imagine what should happen after the customer has been contacted. Should we retry the payment? Should we remember how many times the customer has been contacted because of failed payments? Could there be an alternative path for a payment? Should the order be cancelled? Should we let Shipping know what's happening? Importantly, these are _business decisions_. It is not the use of messaging that requires us to write all the additional logic. Rather, messaging allows us to represent these business decisions and actions as explicit messages, handlers and sagas. We now have the opportunity to implement the requirements transparently, loosely coupled and via autonomous services. Using other approaches, this can easily become very messy, very quickly, perhaps requiring several batch jobs running continuously.

## Advanced Exercise 3.6 : alternative payment providers

The Finance bounded context contacts a payment provider to execute payments. The `Divergent.Finance` project does this in the `InitiatePaymentProcessCommandHandler` class, by calling the `ReliablePaymentClient` class and executing the payment using the `ProcessPayment` method.

Although this is a good and extremely reliable payment provider, it's also very expensive. Business is growing, and it would be prudent to look for alternatives to cut costs. There is another payment provider, but it's less reliable. Obviously this can't get in the way of our payments, but we can at least try to use it. If it fails, we can fallback to the reliable, but more expensive, provider.

Create a new saga in the Finance bounded context which first tries to process the payment with the unreliable payment provider. If that fails (i.e. you don't receive `PaymentSucceededEvent` within the expected time frame), fall back to the reliable provider.

Sagas are not supposed to retrieve data from a data store or call out to external systems. They should execute tasks using [the request/response pattern](http://docs.particular.net/nservicebus/sagas/#sagas-and-request-response). This means that, rather than processing the payment directly, the saga should send a message to another handler in the Finance bounded context to process the payment on its behalf. Whether it fails or succeeds, that handler should [reply to the saga](http://docs.particular.net/nservicebus/messaging/reply-to-a-message) with the status of the payment. If the payment failed, the saga should send another message to process the payment, but this time to the reliable, but expensive, provider.

The solution should end up with at least two new handlers and a new saga. Depending on your solution, you may end up with more.

## Conclusion

This exercise has demonstrated how to use sagas to orchestrate business processes.

If you'd like to discuss this more, please don't hesitate to drop us a line in our [community discussion forum](https://discuss.particular.net/).
