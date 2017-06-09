# Exercise 03 - Sagas

In NServiceBus a saga is the coordinator of a long running (business) process. They are very powerful and once fully grasped, can greatly increase flexibility in projects.

## Introduction

In the last exercise you published events in the `Sales` and `Finance` verticals. The `Shipping`, `Finance` and `Customers` vertical have components that are subscribed to those events. When event arrives, state is stored or updated in the database at each service.

The following events are currently published in the system:
- `OrderSubmittedEvent` - The ordered products are attached to this event, through which we're able to ship them.
- `PaymentSucceededEvent` - The order was successfully paid and we can ship it.

### Business requirement

At the moment we gather all successfully paid orders and the only option we have is to process them in batches. Business is growing rapidly and we would like to process orders as soon as possible, not just at predefined intervals.

### Issues

- After the handlers for both events store data in the database, we need a batch job to gather all successfully paid orders. It is impossible to run this batch job continuously throughout the day, as this will largely increase database locks and interfere with other operations happening in the system. We want to process orders near real-time.

- The events can arrive out-of-order, because the database could be locked or in maintenance mode and the `OrderSubmittedEvent` was set aside to be processed later by Delayed Retries. In the meantime the `PaymentSucceededEvent` might arrive. We need to make sure the order in which we receive these events doesn't matter.

- When more events or actions are added to this business process, the implementation gets scattered across various handlers and classes. We want high cohesion and make sure the implementation makes as much sense as possible and is easy to debug and manage.

## Exercise 03.1 - Replace handlers with a saga

In this exercise you'll replace the two handlers in `Shipping` with a saga. The single saga will handle two events.

**1)** Compile the application to restore all NuGet packages.

**2)** Open the `Divergent.Shipping` project.

**3)** Have a look at the `OrderSubmittedHandler` and `PaymentSucceededHandler` in `Divergent.Shipping` project.   
These are the handlers receiving the accompanying events. The events that can arrive out of order.   
Remove both handlers.

**4)** We're now left with an empty `Handlers` folder. **Rename** it to `Sagas`.

**5)** Add a new class to this folder called `ShippingSaga.cs` and have it implement the `Saga<T>` class. We'll start out with `object` for T and get back later to what it does. You should end up with your class looking like this.
```
    class ShippingSaga : Saga<object>
    {
    }
```

**6)** Now we will have our saga implement both handlers for `OrderSubmittedEvent` and `PaymentSucceededEvent`. Have the class implement the `IAmStartedByMessages<T>` interface for both events.  

NOTE: We're not using `IHandleMessages<T>` to initiate the saga, but rather `IAmStartedByMessages<T>`. The reason is that some messages can start the saga, whereas others should assume the saga to be already instantiated. At least one message should start the saga. We'll get back to this later with more details.

The class should now look like this.
```
    class ShippingSaga : Saga<object>,
            IAmStartedByMessages<OrderSubmittedEvent>,
            IAmStartedByMessages<PaymentSucceededEvent>
    {
    }
```
Override saga base class `ConfigureHowToFindSaga` abstract method and implement both interfaces, leaving methods bodies as is. Before we can implement these, we need to add some saga state.

## Exercise 03.2 - Add state to the saga

A saga is coordinating long-running business processes. The saga you implement, takes decisions based on data that it receives. Imagine a business process that is running for days, years even. Some business processes actually never end. Our saga can't keep all that data in memory forever, so it needs to store this data somewhere.

NOTE: In this exercise, to reduce the system configuration complexity, the configured saga storage is in-memory. In a production environment you would store saga state, and other NServiceBus related data, in a durable persistence storage like SQL-Server, Azure Storage or RavenDB. You can [read more on NServiceBus persistence](http://docs.particular.net/nservicebus/persistence/).

We need to define what state we want to store for our saga. This saga is about a customer's order, so we should store both the `CustomerId` and the `OrderId`. We also want to store the products coming in with the `OrderSubmittedEvent`.

**1)** Define a new class called `ShippingSagaData` and have it inherit from `ContainSagaData`. Add two properties of type `int` for CustomerId and OrderId. You should have a class like this.
```
    class ShippingSagaData : ContainSagaData
    {
        public virtual int OrderId { get; set; }
        public virtual int CustomerId { get; set; }
    }
```

**2)** Now we'll add products. A minor problem is that we can't add `IList<int>` for the products as NServiceBus persisters can't map this properly to tables or documents in SQL-Server and/or RavenDB. We need a complex type. Create a class called `Product`, and in `ShippingSagaData` add a property of `ICollection<Product>` to contain the products. Of course the `Product` class needs to hold the unique id of each ordered product, so we need to add a property for that as well. We'll end up with a class like this.
```
    class ShippingSagaData : ContainSagaData
    {
        public virtual int OrderId { get; set; }
        public virtual int CustomerId { get; set; }
        public virtual ICollection<Product> Products { get; set; }

        public class Product
        {
            public virtual int Identifier { get; set; }
        }
    }
```

NOTE: The saga data implementation utilizes the `virtual` property modifier beacuse saga instances are stored in SQL Server via the `NServiceBus.NHibernate` persister, and NHibernate requires properties to be `virtual`.

**3)** Now we'll add this class as saga state to our saga. We'll end up with a class like this.

```
    class ShippingSaga : Saga<ShippingSagaData>,
            IAmStartedByMessages<OrderSubmittedEvent>,
            IAmStartedByMessages<PaymentSucceededEvent>
    {
    }
```

**4)** Make sure everything compiles.

**5)** Store the `CustomerId`, `OrderId` and products in the state of the saga. We can do this by accessing the saga's `Data` property, as in `Data.CustomerId`. Set the appropriate properties in both `Handle` methods.
```
    public async Task Handle(OrderSubmittedEvent message, IMessageHandlerContext context)
    {
        Data.OrderId = message.OrderId;
        Data.CustomerId = message.CustomerId;
        var projection = message.Products
                                .Select(p => new ShippingSagaData.Product
                                {
                                  Identifier = p
                                });
        Data.Products = projection.ToList();
    }

    public async Task Handle(PaymentSucceededEvent message, IMessageHandlerContext context)
    {
        Data.OrderId = message.OrderId;
    }
```

## Exercise 03.3 - Map incoming messages to the saga

In this exercise you will map incoming messages to the saga so that NServiceBus knows which property to use to find and retrieve the correct saga instance from the saga data storage.

**1)** Open the `ShippingSaga` class and locate the `ConfigureHowToFindSaga` method.

**2)** Map the `OrderSubmittedEvent` property `OrderId` to the sagas `OrderId` property. Do this by overriding The `ConfigureHowToFindSaga` method on the saga. It provides a `mapper` object as an argument. The mapper object exposes a `ConfigureMapping` method, which takes the event type and property to match, as well as the saga property you want to map it to. Map the `PaymentSucceededEvent` property `OrderId` to the sagas `OrderId` property.:

```
mapper.ConfigureMapping<OrderSubmittedEvent>(p => p.OrderId).ToSaga(s => s.OrderId);
mapper.ConfigureMapping<PaymentSucceededEvent>(p => p.OrderId).ToSaga(s => s.OrderId);
```

## Exercise 03.4 - Deal with out-of-order delivery

In this exercise you will process the messages coming in and make sure the messages can arrive in any order, by verifying if all expected messages have been received.

**1)** There are various ways to check in what state the saga is. We can add flags to the saga state to verify which steps already happened.

**2)** To verify if `PaymentSucceededEvent` has been received, we can set a boolean property on the saga state. Add a boolean property to `ShippingSagaData` called `IsPaymentProcessedYet` and in the handler for the `PaymentSuccedeedEvent` we set this property to true.
```
    public async Task Handle(PaymentSucceededEvent message, IMessageHandlerContext context)
    {
        Data.OrderId = message.OrderId;
        Data.IsPaymentProcessedYet = true;
    }
```

NOTE: If the `IsPaymentProcessedYet` property is generated using refactoring tools, it may have a internal setter:

```c#
public bool IsPaymentProcessedYet { get; internal set; }
```

Make sure you remove the `internal` keyword to make the setter public. Otherwise, when `ShippingSagaData` is rehydrated from storage, the property may not be set correctly.

**3)** To verify if `OrderSubmittedEvent` has been received, we can set a boolean property on the saga state. Add a boolean property to `ShippingSagaData` called `IsOrderSubmitted`, and in the handler for the `OrderSubmittedEvent` set this property to true.


NOTE: If the `IsOrderSubmitted` property is generated using refactoring tools, it may have a internal setter:

```c#
public bool IsOrderSubmitted { get; internal set; }
```

Make sure you remove the `internal` keyword to make the setter public. Otherwise, when `ShippingSagaData` is rehydrated from storage, the property may not be set correctly.

The client or service should verify if its a valid order and not have the user be able to submit the order without any products. Commands and events can technically fail, but should not functionally fail because proper validation was not done on the sending side.

Here's how the `ShippingSagaData` class looks with all the required properties created during this exercise:

```c#
class ShippingSagaData : ContainSagaData
{
    public virtual int OrderId { get; set; }
    public virtual int CustomerId { get; set; }
    public virtual ICollection<Product> Products { get; set; }
    public virtual bool IsPaymentProcessedYet { get; set; }
    public virtual bool IsOrderSubmitted { get; set; }

    public class Product
    {
        public virtual int Identifier { get; set; }
    }
}
```

**4)** In the saga add a new async method called `ProcessOrder` that you will call from both `Handle` methods. Inside this method, verify if the saga state property `IsPaymentProcessedYet` was set and if the `IsOrderSubmitted` property was set as well. If yes, invoke `MarkAsComplete()`. This method signals NServiceBus that we're done with this saga and that it can be removed from the underlying storage.

```
    private async Task ProcessOrder(IMessageHandlerContext context)
    {
        if (Data.IsOrderSubmitted && Data.IsPaymentProcessedYet)
        {
            // Send a message to execute shipment
            MarkAsComplete();
        }
    }
```

# Advanced exercises

The advanced exercises are added as additional challenges. They don't have such a detailed guidance and we didn't provide solution for them. If you feel like it, you can do them during the workshop, in your hotel or at home. If you have questions you can ask them during the workshop or using Particular Software free support channel on the Google Groups: https://groups.google.com/forum/#!forum/particularsoftware.

## Advanced Exercise 01
This exercise does not require NServiceBus skills, but you will probably need to read documentation on how to finish this.

Sagas are excellent for coordinating a business process. In the current saga we are only able to execute the happy flow of the business process. The flow where everything goes well. Payment succeeds and arrives within a short time. But what if the `PaymentSucceededEvent` never arrives?

We can request a timeout in our saga, so that after a certain amount of time we can take action. Like notifying the shipping or sales department. In the real world, a saga in the Finance bounded context would take care of this, but let's also take some action after the timeout elapses. Otherwise this saga would live forever without anyone noticing the order processing failed in some way.

Read more on [saga timeouts here](http://docs.particular.net/nservicebus/sagas/timeouts).


## Advanced Exercise 02
This is an exercise at expert level.

The Finance bounded context is contacting a payment provider to execute the payment. The project `Divergent.Finance` does this in the `InitiatePaymentProcessCommandHandler` class, by calling the `ReliablePaymentClient` class and executing the payment using the `ProcessPayment` method.

Although this is a good and extremely reliable payment provider, it's also very expensive. Business is growing, but it would be foolish to not look for alternatives to cut costs. There is another payment provider, but it's less reliable. Obviously this can't get in the way of our payments, but we can at least try to use it. And if it fails, fallback to the more expensive, but reliable provider.

**Exercise:**
Create a new saga in the Finance bounded context that first tries to process the payment with the unreliable payment provider. If it fails call (i.e. you don't receive `PaymentSucceededEvent` within the expected time frame) then fall back to the reliable provider.

Sagas are not supposed to retrieve data from a data store or call out to external systems. They should retrieve data or execute tasks [using request/response](http://docs.particular.net/nservicebus/sagas/#sagas-and-request-response) pattern. This means that rather than processing the payment directly, the saga should request another handler in the Finance bounded context to process the payment on its behalf. Failing or succeeding, this handler should [reply to the saga](http://docs.particular.net/nservicebus/messaging/reply-to-a-message) with the status of the payment. If the payment failed, the saga should do another request for processing the payment, but this time to the expensive, but reliable payment provider.

The solution should end up with two new handlers and a saga, but depending on your solution you might end up with more.

### Solution configuration

#### Start-up projects

* Divergent.Customers
* Divergent.Customers.API
* Divergent.Finance
* Divergent.Finance.API
* Divergent.Frontend
* Divergent.Sales
* Divergent.Sales.API
* Divergent.Shipping
* PaymentProviders
