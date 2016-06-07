# Exercise 03 - Sagas

In NServiceBus a saga is the coordinator of a long running (business) process. They are very powerful and once fully grasped, can bring endless flexibility and possibilities to your projects.

## Introduction
In the last exercise we published events in the `Sales` and `Finance` bounded contexts. The `Shipping` bounded context has components that are subscribed to these events. When the event arrives, we store some state in the database.

- `OrderSubmittedEvent` - The ordered products are attached to this event, through which we're able to ship them.
- `PaymentSucceededEvent` - The ordered products were successfully paid for and we can ship them.

### Business requirement

At the moment we gather all successfully paid orders at midnight and start processing them. This is the reason we promise customers that orders made before 12am are shipped the next business day. Business is growing rapidly and we would like to process orders as soon as possible, not just at midnight. 

### Issues

- After the handlers for both events store data in the database, we need a batch job to gather all successfully paid orders. It is impossible to run this batch job continuously throughout the day, as this will largely increase database locks and interfere with normal processing. We want to process orders more near real-time.

- The events can arrive out-of-order, because the database could be locked or in maintenance mode and the `OrderSubmittedEvent` was set aside to be processed later by Second Level Retries. In the meantime the `PaymentSucceededEvent` might arrive. We need to make sure the order in which we receive these events does not matter.
 
- When more events or actions are added to this business process, the implementation gets scattered across various handlers and classes. We want high cohesion and make sure the implementation makes as much sense as possible and is easy to debug.

## Exercise 03.1 - Replace handlers by saga

In this exercise we'll replace the two handlers in `Shipping` with the initial layout for the saga we're building. Instead of two sagas we'll create a single saga responding to both events.

**1)** Compile the application to retrieve all NuGet packages.

**2)** Open the `Divergent.Shipping` project.

**3)** Have a look at the `OrderSubmittedHandler` and `PaymentSucceededHandler`.   
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

NOTE: We're not using `IHandleMessages<T>` to initiate the saga, but rather `IAmStartedByMessages<T>`. The reason is that some messages can start the saga, whereas others should assume the saga to be already instantiated. At least one message should start the saga. We'll get back to this later with more information.

The class should now look like this.
```
    class ShippingSaga : Saga<object>,
            IAmStartedByMessages<OrderSubmittedEvent>,
            IAmStartedByMessages<PaymentSucceededEvent>
    {
    }

```
The saga now contains an abstract method called `ConfigureHowToFindSaga` and handle methods for the two events that we need to implement. But before we implement these, we'll have to add some saga state.

## Exercise 03.2 - Add state to the saga

A saga is coordinating long-running business processes. The saga you implement, takes decisions based on data that it receives. But imagine a business process running for days, years even. Some business processes actually never end. Our saga can't keep all that data in memory forever, so it needs to store this data somewhere.

For this exercise we store everything in-memory, but normally you would store this inside a durable persistence like SQL-Server, Azure Storage or RavenDB. You can [read more on NServiceBus persistence](http://docs.particular.net/nservicebus/persistence/).

We need to define what state we want to store for our saga. This saga is about a customer's order, so we should store both the `CustomerId` and the `OrderId`. We also want to store the products coming in with the `OrderSubmittedEvent`.

**1)** Define a new class called `ShippingSagaData` and have it inherit from `ContainSagaData`. Add two properties of type `Guid` for CustomerId and OrderId. You should have a class like this.
```
    class ShippingSagaData : ContainSagaData
    {
        public Guid OrderId { get; set; }
        public Guid CustomerId { get; set; }
    }
``` 

**2)** Now we'll add the products. A minor problem is that we can't add `IList<Guid>` for the products as NServiceBus can't map this properly to tables or documents in SQL-Server and/or RavenDB. We need an additional complex type. So create a class called `Product`, and in the `ShippingSagaData` we'll add a property of `IList<Product>` to contain the products. Of course the `Product` class needs to hold the unique id of the products, so we need to add a property for that as well. We'll end up with a class like this.
```
    class ShippingSagaData : ContainSagaData
    {
        public Guid OrderId { get; set; }
        public Guid CustomerId { get; set; }
        public ICollection<Product> Products { get; set; }

        public class Product
        {
            public Guid Identifier { get; set; }
        }
    }
```

**3)** Now we'll add this class as saga state to our saga. We'll end up with a class like this.
```
    class ShippingSaga : Saga<ShippingSagaData>,
            IAmStartedByMessages<OrderSubmittedEvent>,
            IAmStartedByMessages<PaymentSucceededEvent>
    {
    }
```

**4)** Next implement the abstract method and interfaces. Make sure everything compiles.

**5)** Store the `CustomerId`, `OrderId` and products in the state of the saga. We can do this by accessing the saga its property `Data`, as in `Data.CustomerId`. Copy over the appropriate properties in both `Handle` methods.

**6)** Try to finish this exercise. If you get stuck, you can find the `ShippingSaga` and `ShippingSagaData` classes at the bottom of this article in paragraph `Exercise 03.2 - solution`.


## Exercise 03.3 - Map incoming messages to the saga
In this exercise we will map incoming messages to the saga so NServiceBus knows which property to use to find the correct saga instance in the database.

**1)** Open the `ShippingSaga` class and find the `ConfigureHowToFindSaga` method.

**2)** Map the `OrderSubmittedEvent` property `OrderId` to the sagas `OrderId` property. Do this by overriding The `ConfigureHowToFindSaga` method on the saga. It provides a `mapper` object as an argument. The mapper object exposes a `ConfigureMapping` method, which takes the event type and property to match as well as the saga property you want to map it to. Like this:

```
mapper.ConfigureMapping<OrderSubmittedEvent>(msg => msg.OrderId).ToSaga(sagaData => sagaData.OrderId);
```

**3)** Map the `PaymentSucceededEvent` property `OrderId` to the sagas `OrderId` property.

**4)** Try to finish this exercise. If you get stuck, you can find the method at the bottom of this article in paragraph `Exercise 03.2 - solution`.

## Exercise 03.4 - Deal with out-of-order delivery
In this exercise we will process the messages coming in and make sure the messages can arrive in any order, by simply verifying if both messages have been delivered.

**1)** There are various ways to check in what state the saga is. We can add flags to the saga state to verify which steps have occurred. But we can also use already set state to find the current state in a more natural way.

**2)** To verify if `PaymentSucceededEvent` has arrived, we can't do anything else but set a boolean property on the saga state. Add a boolean property to `ShippingSagaData` called `IsPaymentProcessedYet` and in the handler for the `OrderSubmittedEvent` we set this property to true.

**3)** If the `OrderSubmittedEvent` was processed, the `Products` property of the saga state should have at least one item. The client or service should verify if its a valid order and not have the user be able to submit the order without any products. Commands and events can technically fail, but should not fail because proper validation was not done on the sending side. We can conclude that we don't have to set an additional property here.

**4)** Add a new method called `ProcessOrder` and make it async and return `Task`.

**5)** Inside this method, verify if the saga state property `IsPaymentProcessedYet` was set and if the `Products` property contains more than one item. If it does, call `MarkAsComplete();`. This method signals NServiceBus that we're done with this saga and that it can remove the saga state from storage.

**6)** Try to finish this exercise. If you get stuck, you can find the method at the bottom of this article in paragraph `Exercise 03.4 - solution`.

In exercise 04 for integration, we will add more logic to this message, so that it makes more sense to complete this saga and continue the process.

# Advanced exercises
The advanced exercises are added as additional challenges. They are without guidance and even without a solution. If you feel like it, you can do them during the workshop, in your hotel or at home. If you have questions you can ask them during the workshop or using Particular Software its free support channel on the Google Groups : https://groups.google.com/forum/#!forum/particularsoftware 

## Advanced Exercise 01
This exercise does not require NServiceBus skills, but you will probably need to read documentation on how to finish this.

Sagas are excellent for coordinating the business process. In the current saga we are only able to execute the happy flow of the business process. The flow where everything goes well. Payment succeeds and arrives within a short time. But what if the `PaymentSucceededEvent` never arrives?

We can request a timeout in our saga, so that after a certain amount of time we can take action. Like notifying the shipping or sales department. In the real world, a saga in the Finance bounded context would take care of this, but let's implement here as well to take some action. Otherwise this saga would live forever without anyone actually knowing.

Read more on [saga timeouts here](http://docs.particular.net/nservicebus/sagas/timeouts).


## Advanced Exercise 02
This is an exercise at expert level.

The Finance bounded context is contacting a payment provider to execute the payment. The project `Divergent.Finance` does this in the `OrderSubmittedHandler` class. It's calling the `ReliablePaymentClient` class and executing the payment using the `ProcessPayment` method.

The thing is, although this is a good and extremely reliable payment provider, it's also very expensive. Business is growing, but it would be foolish to not look for alternatives to cut costs. We've found another payment provider, but it's less reliable. Obviously this can't get in the way of our payments, but we can at least try to use it. And if it fails, fallback to the expensive, but reliable provider.

**Exercise:**
Try to build a saga in the Finance bounded context that first tries the unreliable payment provider. If it fails call the reliable provider. Sagas however are not supposed to retrieve data from a datastore or call out to external systems. They should retrieve data or execute tasks [using request/response](http://docs.particular.net/nservicebus/sagas/#sagas-and-request-response). This means the saga should request another handler in the Finance bounded context, to actually process the payment. Failing or succeeding, this handler should [reply to the saga](http://docs.particular.net/nservicebus/messaging/reply-to-a-message) with the status of the payment. If the payment failed, the saga should do another request for processing the payment, but this time to the expensive, but reliable payment provider.

The solution should end up with two new handlers and a saga, but depending on your solution you might end up with more.

# Solutions

## Exercise 03.2 - solution

```
    class ShippingSaga : Saga<ShippingSagaData>,
            IAmStartedByMessages<OrderSubmittedEvent>,
            IAmStartedByMessages<PaymentSucceededEvent>
    {
        protected override void ConfigureHowToFindSaga(SagaPropertyMapper<ShippingSagaData> mapper)
        {
        }

        public async Task Handle(OrderSubmittedEvent message, IMessageHandlerContext context)
        {
            Data.OrderId = message.OrderId;
            Data.CustomerId = message.CustomerId;
            Data.Products = message.Products.Select(p => new ShippingSagaData.Product() {Identifier = p}).ToList();
        }

        public async Task Handle(PaymentSucceededEvent message, IMessageHandlerContext context)
        {
            Data.OrderId = message.OrderId;
        }
    }

    class ShippingSagaData : ContainSagaData
    {
        public Guid OrderId { get; set; }
        public Guid CustomerId { get; set; }
        public ICollection<Product> Products { get; set; }

        public class Product
        {
            public Guid Identifier { get; set; }
        }
    }
```

## Exercise 03.3 - solution
```
    protected override void ConfigureHowToFindSaga(SagaPropertyMapper<ShippingSagaData> mapper)
    {
        mapper.ConfigureMapping<OrderSubmittedEvent>(msg => msg.OrderId).ToSaga(sagaData => sagaData.OrderId);
        mapper.ConfigureMapping<PaymentSucceededEvent>(msg => msg.OrderId).ToSaga(sagaData => sagaData.OrderId);
    }
```

## Exercise 03.4 - solution
```
    class ShippingSaga : Saga<ShippingSagaData>,
            IAmStartedByMessages<OrderSubmittedEvent>,
            IAmStartedByMessages<PaymentSucceededEvent>
    {
    protected override void ConfigureHowToFindSaga(SagaPropertyMapper<ShippingSagaData> mapper)
    {
        mapper.ConfigureMapping<OrderSubmittedEvent>(msg => msg.OrderId).ToSaga(sagaData => sagaData.OrderId);
        mapper.ConfigureMapping<PaymentSucceededEvent>(msg => msg.OrderId).ToSaga(sagaData => sagaData.OrderId);
    }

        public async Task Handle(OrderSubmittedEvent message, IMessageHandlerContext context)
        {
            Data.OrderId = message.OrderId;
            Data.CustomerId = message.CustomerId;
            Data.Products = message.Products.Select(p => new ShippingSagaData.Product() {Identifier = p}).ToList();

            await ProcessOrder();
        }

        public async Task Handle(PaymentSucceededEvent message, IMessageHandlerContext context)
        {
            Data.OrderId = message.OrderId;
            Data.IsPaymentProcessedYet = true;

            await ProcessOrder();

        }

        private async Task ProcessOrder()
        {
            if (Data.Products.Count > 0 && Data.IsPaymentProcessedYet)
            {
                // Send a message to execute shipment
                MarkAsComplete();
            }
        }
    }
```

### Solution configuration

#### Start-up projects

* Divergent.Customers
* Divergent.Customers.API
* Divergent.Finance
* Divergent.Finance.API
* Divergent.Frontend.SPA
* Divergent.Sales
* Divergent.Sales.API
* Divergent.Shipping
* PaymentProviders
