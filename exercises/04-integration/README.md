# Exercise 4: Integration

**Important: Before attempting the exercise, please ensure you have followed [the instructions for preparing your machine](/README.md#preparing-your-machine-for-the-workshop) and that you have read [the instructions for running the exercise solutions](/README.md#running-the-exercise-solutions).**

You will quickly run into the need for integration with 3rd party systems. You will use Web Services, REST based APIs, you'll emails or export files. Most of the time data need to be retrieved from several different services before interacting with a 3rd party.

Use a specific endpoint called IT/Ops for integration. It defines a set of interfaces for getting the data needed to integrate with 3rd parties. Services will implement those IT/Ops interfaces. Each service needs to provide the functionality for accessing its own data to avoid breaking service autonomy. Each service deploys its providers to the IT/Ops endpoint, which will co-host all of them and perform the 3rd party integration. It is important to note that IT/Ops has no dependencies on any service. It only defines provider interfaces and uses implementations from services after deployment at run time.

## Overview

In the last exercise, we created a saga in Shipping to wait for `OrderSubmittedEvent` and `PaymentSucceededEvent`. We now want the saga to send a `ShipWithFedexCommand` to IT/Ops so that FedEx will pick up and ship the package.

IT/Ops needs to respond to that message by calling implementations of its `IProvideCustomerInfo` and `IProvideShippingInfo` interfaces. Customers service owns customer data, so it will provide an implementation of `IProvideCustomerInfo`. Shipping service holds information regarding weight and volume of packages, so it will provide an implementation of `IProvideShippingInfo`.

A note about deployment: In a production environment one would package each provider into a deployable artifact (e.g. a NuGet package) and deploy them to the IT/Ops endpoint. In this exercise for simplicity we will use post-build events in `Divergent.Customers.Data` and `Divergent.Shipping.Data` to copy providers implementations into IT/Ops.

## Start-up projects

For more info, please see [the instructions for running the exercise solutions](/README.md#running-the-exercise-solutions).

* Divergent.CompositionGateway
* Divergent.Customers
* Divergent.Customers.API
* Divergent.Finance
* Divergent.Finance.API
* Divergent.Frontend
* Divergent.ITOps
* Divergent.Sales
* Divergent.Sales.API
* Divergent.Shipping
* PaymentProviders

## Business requirements

Once an order is ready to be shipped, we want to initiate the shipping process by telling our shipping partner to pick up the package. We do this by calling their Web Service with the customer name, address, weight and volume of the package to ship.

## Exercise 4.1: send and handle integration command from Shipping to IT/Ops

In this exercise, we'll have the saga in `Divergent.Shipping` service tell IT/Ops to integrate with FedEx. The saga will do this by sending a command to IT/Ops when it knows an order has been placed and payment received. IT/Ops will fetch data through providers and call FedEx.

### Step 1

In the `Divergent.ItOps.Messages` project, create a new class `ShipWithFedexCommand` in the Commands folder. It should contain the order Id, customer Id, and a list of product Ids.

```c#
public class ShipWithFedexCommand
{
    public int OrderId { get; set; }
    public int CustomerId { get; set; }
    public List<int> Products { get; set; }
}
```

### Step 2

In the `Divergent.Shipping` project, open `ShippingSaga.cs` and look at the `ProcessOrder` method. The method should check if the order has been both submitted (`Data.IsOrderSubmitted`) and paid for (`Data.IsPaymentProcessed`). If so, it should send the new `ShipWithFedexCommand`.

```c#
private async Task ProcessOrder(IMessageHandlerContext context)
{
    if (Data.IsOrderSubmitted && Data.IsPaymentProcessed)
    {
        await context.Send(new ShipWithFedexCommand
        {
            OrderId = Data.OrderId,
            CustomerId = Data.CustomerId,
            Products = Data.Products.Select(s => s.Identifier).ToList(),
        });

        MarkAsComplete();
    }
}
```

### Step 3

In the `Divergent.Shipping` project, configure the destination endpoint for the `ShipWithFedexCommand`. To do this use the `routing` object obtained when configuring the transport and add the following statement

```
routing.RouteToEndpoint(typeof(ShipWithFedexCommand), "Divergent.ITOps");
```

### Step 4

In the `Divergent.ITOps` project, add a class under Handlers called `ShipWithFedexCommandHandler`. It should contain a message handler for `ShipWithFedexCommand` that calls a fake FedEx Web Service. Hard-code the customer information for now.

```c#
public class ShipWithFedexCommandHandler : IHandleMessages<ShipWithFedexCommand>
{
    private static readonly ILog Log = LogManager.GetLogger<ShipWithFedexCommandHandler>();

    public async Task Handle(ShipWithFedexCommand message, IMessageHandlerContext context)
    {
        Log.Info("Handle ShipWithFedexCommand");
        var name = "John Balder";
        var street = "212 Regent Street";
        var city = "London";
        var postalCode = "123";
        var country = "UK";

        var fedExRequest = CreateFedexRequest(name, street, city, postalCode, country);
        await CallFedexWebService(fedExRequest);
        Log.Info($"Order {message.OrderId} shipped with Fedex");
    }

    private XDocument CreateFedexRequest(string name, string street, string city, string postalCode, string country)
    {
        var shipment = 
            new XDocument(
                new XElement("FedExShipment",
                    new XElement("ShipTo",
                        new XElement("Name", name),
                        new XElement("Street", street),
                        new XElement("City", city),
                        new XElement("PostalCode", postalCode),
                        new XElement("Country", country))));
        return shipment;
    }

    private Task CallFedexWebService(XDocument fedExRequest)
    {
        //do web service call etc.
        return Task.CompletedTask;
    }
}
```

### Step 5

Run the solution and verify that the IT/Ops message handler is invoked whenever you submit a new order in the UI.

## Exercise 4.2: implement customer provider

In this exercise, we'll implement the customer provider in the Customers service and make IT/Ops use it to fetch the customer information.

### Step 1

The `Divergent.ITOps.Interfaces` project, have a look at `IProvideCustomerInfo.cs`. The interface has a single method that takes a customer ID and returns a `CustomerInfo` instance with name, street, etc. for the specified customer.

### Step 2

In the `Divergent.Customers.Data` project, add a new class called `CustomerInfoProvider` in the ITOps folder. It should implement the `IProvideCustomerInfo` interface from `Divergent.ITOps.Interfaces`. 

```c#
public class CustomerInfoProvider : IProvideCustomerInfo
{
    public async Task<CustomerInfo> GetCustomerInfo(int customerId)
    {
        using (var db = new Context.CustomersContext())
        {
            var customer = await db.Customers.Where(c => c.Id == customerId).SingleAsync();

            return new CustomerInfo
            {
                Name = customer.Name,
                Street = customer.Street,
                City = customer.City,
                PostalCode = customer.PostalCode,
                Country = customer.Country,
            };
        }
    }
}
```

NOTE: `SingleAsync()` is an asynchronous Entity Framework LINQ extension in the `System.Data.Entity` namespace. Ensure you add an appropriate `using` statement.

### Step 3

Go to properties on the `Divergent.Customers.Data` project. Double check that it contains a post-build event:

 ```bat
 copy /Y "$(TargetDir)$(ProjectName).dll" "$(SolutionDir)Divergent.ITOps\Providers\$(ProjectName).dll"
 ```

This "deploys" the provider into a location that IT/Ops knows. IT/Ops doesn't need a direct reference to other services.

### Step 4

Open `ShipWithFedexCommandHandler.cs` in the `Divergent.ITOps`. This class should take a constructor dependency on `IProvideCustomerInfo`. The Dependency Injection framework will find the provider implementations as long as they are copied to IT/Ops. The handler should use this dependency to fetch the customer information instead of using the hard-coded values.

```c#
public class ShipWithFedexCommandHandler : IHandleMessages<ShipWithFedexCommand>
{        
    private readonly IProvideCustomerInfo _customerProvider;
    private static readonly ILog Log = LogManager.GetLogger<ShipWithFedexCommandHandler>();

    public ShipWithFedexCommandHandler(IProvideCustomerInfo customerProvider)
    {            
        _customerProvider = customerProvider;
    }

    public async Task Handle(ShipWithFedexCommand message, IMessageHandlerContext context)
    {
        Log.Info("Handle ShipWithFedexCommand");

        var customerInfo = await _customerProvider.GetCustomerInfo(message.CustomerId);

        var fedExRequest = CreateFedexRequest(customerInfo);
        await CallFedexWebService(fedExRequest);
        Log.Info($"Order {message.OrderId} shipped with Fedex");
    }

    private XDocument CreateFedexRequest(CustomerInfo customerInfo)
    {
        var shipment =
            new XDocument(
                new XElement("FedExShipment",
                    new XElement("ShipTo",
                        new XElement("Name", customerInfo.Name),
                        new XElement("Street", customerInfo.Street),
                        new XElement("City", customerInfo.City),
                        new XElement("PostalCode", customerInfo.PostalCode),
                        new XElement("Country", customerInfo.Country))));

        return shipment;
    }

    private Task CallFedexWebService(XDocument fedExRequest)
    {
        //do web service call etc.
        return Task.CompletedTask;
    }
}
```

### Step 5

Run the solution and verify that the ITOps message handler fetches the customer information using the supplied provider.

NOTE: For your convenience IT/Ops is already configured with the required connection strings to allow providers to function properly.

Check out `ReflectionHelper` and look at how the container is created in the `Host` in `Divergent.ITOps` to learn more about how IT/Ops loads and co-hosts the providers.


## Exercise 4.3: implement shipping provider

In this exercise, you'll implement the shipping provider in the Shipping service. ITOps defines an `IProvideShippingInfo` interface. The provider will be implemented in the Shipping service using the two helpers `VolumeEstimator` and `WeightCalculator` already present there.

### Step 1

In the `Divergent.Shipping.Data` project, add a new class named `ShippingInfoProvider` in the ITOps folder. It should implement the `IProvideShippingInfo` interface from `Divergent.ITOps.Interfaces`.  The implementation should use `VolumeEstimator` and `WeightCalculator` to implement the `GetPackageInfo` method.

```c#
public class ShippingInfoProvider : IProvideShippingInfo
{
    public Task<PackageInfo> GetPackageInfo(IEnumerable<int> productIds)
    {
        var count = productIds.Count();

        return Task.FromResult(new PackageInfo()
        {
            Weight = WeightCalculator.CalculateWeight(count),
            Volume = VolumeEstimator.Calculate(count)
        });
    }
}
```

### Step 2

Go to the `Divergent.Shipping.Data` project properties. Double check that it contains a post-build event:

```batch
copy /Y "$(TargetDir)$(ProjectName).dll" "$(SolutionDir)Divergent.ITOps\Providers\$(ProjectName).dll"
```

### Step 3

Open `ShipWithFedexCommandHandler.cs` in `Divergent.ITOps`. It should now also take a constructor dependency on `IProvideShippingInfo`.

```c#
public class ShipWithFedexCommandHandler : IHandleMessages<ShipWithFedexCommand>
{
    private readonly IProvideShippingInfo _shippingProvider;
    private readonly IProvideCustomerInfo _customerProvider;
    private static readonly ILog Log = LogManager.GetLogger<ShipWithFedexCommandHandler>();

    public ShipWithFedexCommandHandler(IProvideShippingInfo shippingProvider, IProvideCustomerInfo customerProvider)
    {
        _shippingProvider = shippingProvider;
        _customerProvider = customerProvider;
    }

    public async Task Handle(ShipWithFedexCommand message, IMessageHandlerContext context)
    {
        Log.Info("Handle ShipWithFedexCommand");

        var shippingInfo = await _shippingProvider.GetPackageInfo(message.Products);
        var customerInfo = await _customerProvider.GetCustomerInfo(message.CustomerId);

        var fedExRequest = CreateFedexRequest(shippingInfo, customerInfo);
        await CallFedexWebService(fedExRequest);
        Log.Info($"Order {message.OrderId} shipped with Fedex");
    }

    private XDocument CreateFedexRequest(PackageInfo packageInfo, CustomerInfo customerInfo)
    {
        var shipment =
            new XDocument(
                new XElement("FedExShipment",
                    new XElement("ShipTo",
                        new XElement("Name", customerInfo.Name),
                        new XElement("Street", customerInfo.Street),
                        new XElement("City", customerInfo.City),
                        new XElement("PostalCode", customerInfo.PostalCode),
                        new XElement("Country", customerInfo.Country)),
                    new XElement("Measurements",
                        new XElement("Volume", packageInfo.Volume),
                        new XElement("Weight", packageInfo.Weight))));
        return shipment;
    }

    private Task CallFedexWebService(XDocument fedExRequest)
    {
        //do web service call etc.
        return Task.CompletedTask;
    }
}
```

### Step 4

Run the solution and verify that the IT/Ops message handler now fetches both the customer and shipping information via the providers.

NOTE: It might happen that the exercise fails at run time with an `Autofac` exception, similar to the following:

```
Autofac.Core.DependencyResolutionException: None of the constructors found with 'Autofac.Core.Activators.Reflection.DefaultConstructorFinder' on type 'Divergent.ITOps.Handlers.ShipWithFedexCommandHandler' can be invoked with the available services and parameters:
Cannot resolve parameter 'Divergent.ITOps.Interfaces.IProvideShippingInfo shippingProvider' of constructor 'Void .ctor(Divergent.ITOps.Interfaces.IProvideShippingInfo, Divergent.ITOps.Interfaces.IProvideCustomerInfo)'.
```

To keep the example and 'deployment' simple, a post-build event is used to copy the binary assembly to the `IT/Ops` folder. Via reflection the assembly is loaded and registered in `Autofac`, the configured dependency injection container.

Due to various reasons, these files might be missing and the aforementioned error might occur for either the `IProvideCustomerInfo` or `IProvideShippingInfo` parameters in the constructor of the `ShipWithFedexCommandHandler` class.

Do a full rebuild of the entire solution using `CTRL+SHIFT+B`

## Advanced exercise 4.4: visualizing the system

**Important: Before attempting the advanced exercises, please ensure you have followed [the instructions for preparing your machine for the advanced exercises](/README.md#preparing-your-machine-for-the-advanced-exercises).**

If you finished the advanced exercises in module 2, you've seen how ServicePulse can monitor and inform us about the status of our endpoints. In this module we'll have a look at how ServiceInsight visualizes the system.

### Step 1

Open ServiceInsight from the Windows Start menu.

### Step 2

ServiceInsight connects to ServiceControl to retrieve information about all endpoints and messages. If ServiceInsight is yet not connected to ServiceControl, the top-left icon with the tooltip 'Connect Endpoint Explorer to ServiceControl instance' allows you to connect to ServiceControl. The default address is `http://localhost:33333/api/`.

NOTE: The ServiceControl Management tool allows the creation of multiple instances of ServiceControl, each using a different port.

### Step 3

After connecting to ServiceControl, the "Endpoint Explorer" on the left side of ServiceInsight shows a top level node representing the ServiceControl instance (showing the URL) containing a node for every endpoint which is sending audit messages to ServiceControl. Click on the either the top level node or one of the endpoints nodes. To the right, at the top of the window, the "Messages" shows the messages themselves.

Below the message list you will see a flow diagram for the most recent message. The currently selected message is visible in the flow diagram by a thick border around the message.

When you select another message in the message list, the flow diagram will update accordingly. This also works the other way around. When you click on message in the flow diagram, the corresponding mesage will be selected in the messages list.

### Step 4

To the right of the flow diagram you will see "Message Properties" showing the properties of the currently selected message, such as:

- The type, unique message ID, and conversation ID of the message.
- Performance details on when the message was sent, how long it took to process it, and how long the critical time was, which is the time from sending it until successfully processing it.
- If the message wasn't successfully processed, the complete stacktrace of the exception is shown under "Errors", "ExceptionInfo". If the stacktrace includes a large amount of redundant information, e.g. async invocation information, this can be cleaned up using the method shown in [this sample](https://docs.particular.net/samples/logging/stack-trace-cleaning/).

### Step 5

At the bottom of the flow diagram, you will see tabs which allow switching to other views.

The "Saga" will be empty. This will be the case when the currently selected message was not handled by a saga. Even when the message was handled by a saga the endpoint needs to have the saga auditing plugin installed before the saga view will be populated.

## Advanced exercise 4.5: enable saga auditing

In this exercise, we'll set up every endpoint to send saga audit messages to ServiceControl, which allow us to visualize sagas in ServiceControl.

### Step 1

If you are not familiar with the saga audit plugin, you will need to read the [documentation](https://docs.particular.net/nservicebus/sagas/saga-audit?version=sagaaudit_3) for how to install it first.

To install the plugin type: `Install-Package NServiceBus.SagaAudit -Version 3.0.0`, in Package Manager Console.

If you use `Manage Nuget Packages` option, make sure you select **version 3.0.0**

You only need to install the plugin in the `Divergent.Shipping` project, since it is the only project containing a saga at the moment. If you've added sagas to other endpoints, don't forget to install the plugin there as well.

Don't forget to configure the endpoint to tell it where to send saga audit messages to.

### Step 2

Start up the solution and create another order using the web interface.

Once messages start arriving in the saga, additional messages will be sent to ServiceControl which contain more detailed information on what happened within the saga.

### Step 3

In ServiceInsight, both the flow diagram as the saga view should display additional data.

The flow diagram will now show which messages initiated, updated or completed the saga.

The saga view will now display all incoming and outgoing messages.

## Advanced exercise 4.6: errors and saga timeouts

This exercise is simliar to what you saw in the advanced exercises for module 2, but this time with sagas.

### Step 1

Add the throwing of an exception to a saga handler method, which will prevent the saga from progressing any further. This means our saga won't receive and process the messages it requires to be completed, and it will continue to exist indefinitely.

After the failed messages arrive in ServiceControl, you should see them appear in ServicePulse and ServiceInsight. You can now retry the messages from either user interface. If you remove the throwing of the exception, the messages should be processed normally and the saga should be completed.

### Step 2

Imagine the message indicating successful payment is never received by the saga. Should we wait forever for this message? Perhaps we could ask the business how long we should wait and what to do if we don't recieve a message in that time.

Perform the action from step 1 again, throwing an exception. But this time, send a timeout message from the saga. In this exercise we'll complete the saga, taking the action that business tells us should happen when the payment is not received in the alloted time. Possible actions could be sending an email to finance to take action, cancelling the order, or asking the customer to manually pay the invoice via our website.

Summary of actions to take:

- Throw an exception so the payment will never succeed
- Send a timeout message in the saga, with more information about [how to in our documentation](https://docs.particular.net/nservicebus/sagas/timeouts).
- Process the timeout message and complete the saga (perhaps with a comment showing that _some_ business defined action would be iniated at this point if this were a production system).
- Check ServiceInsight to see how this is visualized.

### Step 3

Consider the following items and perhaps discuss them with your colleagues:

- Have you ever seen a business process with similar characteristics to the above example? Such as waiting forever for some action to happen, which had already failed elsewhere. It's usually easy to think of multiple examples from any non-trivial business process.
- How did you deal with those? Did you have to run batch jobs, or connect directly to the database to hack the data manually? Or some other complicated workaround?
- When this happened? Were you able to ask the business how to handle the failures and then design the system to match the solution described by the business? Or did you have to translate the business language into a complex set of technical operations which approximated the business language?

## Conclusion

This exercise has demonstrated how to address integration issues where, as with the UI, data from multiple services needs to be combined in order to perform some action. We also saw, again, how powerful sagas can be in orchestrating business processes.

If you'd like to discuss this more, please don't hesitate to drop us a line in our [community discussion forum](https://discuss.particular.net/).
