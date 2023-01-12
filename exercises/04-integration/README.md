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

In the `Divergent.ItOps.Messages` project, create a folder `Commands` and add a new class `ShipWithFedexCommand` in the Commands folder. It should contain the order Id, customer Id, and a list of product Ids.

```c#
﻿using System.Collections.Generic;

namespace Divergent.ITOps.Messages.Commands
{
    public class ShipWithFedexCommand
    {
        public int OrderId { get; set; }
        public int CustomerId { get; set; }
        public List<int> Products { get; set; }
    }
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
            Products = Data.Products
        });

        MarkAsComplete();
    }
}
```

### Step 3

In the `Divergent.Shipping` project, configure the destination endpoint for the `ShipWithFedexCommand`. To do this use the `endpoint` object for configuring NServiceBus and add the following statement

```
endpoint.Configure(routing =>
{
    routing.RouteToEndpoint(typeof(ShipWithFedexCommand), "Divergent.ITOps");
});
```

### Step 4

In the `Divergent.ITOps` project, create a folder `Handlers` and add a class called `ShipWithFedexCommandHandler`. It should contain a message handler for `ShipWithFedexCommand` that calls a fake FedEx Web Service. Hard-code the customer information for now.

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
    private readonly ILiteDbContext db;

    public CustomerInfoProvider(ILiteDbContext db) => this.db = db;

    public CustomerInfo GetCustomerInfo(int customerId)
    {
        var customer = db.Database.GetCollection<Customer>().Query().Where(c => c.Id == customerId).First();

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
```

### Step 3

The `CustomerInfoProvider` class that you just created is owned by the customer service. However `ITOps` is responsible for contacting third parties like FedEx.

In a real world scenario you would create a deployment step in your CI pipeline (in Azure DevOps, TeamCity, GitHub workflow, Octopus, etc) where you deploy this class together with the `ITOps` project. Using reflection, `ITOps` will then search for implementations of the `IProvideCustomerInfo` class and execute those before connecting to FedEx and sending the required data over.

In this exercise however, we created a project reference in `Divergent.ITOps` to `Divergent.Customers.Data` to include the assembly in the execution folder, as it would be too complex to set up the deployment pipeline.

You can also check out how we included reflection to find and register the `LiteDbContext` from `Divergent.Customers.Data`. This is done using reflection in `Program.cs` of `Divergent.ITOps` by searching for the `IRegisterServices` in the  `ServiceRegistration` class.

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

        var customerInfo = _customerProvider.GetCustomerInfo(message.CustomerId);

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

Check out `ReflectionHelper` and look at how the container is created in the `Host` in `Divergent.ITOps` to learn more about how IT/Ops loads and co-hosts the providers.

## Exercise 4.3: implement shipping provider

In this exercise, you'll implement the shipping provider in the Shipping service. ITOps defines an `IProvideShippingInfo` interface. The provider will be implemented in the Shipping service using the two helpers `VolumeEstimator` and `WeightCalculator` already present there.

### Step 1

In the `Divergent.Shipping.Data` project, add a new class named `ShippingInfoProvider` in the ITOps folder. It should implement the `IProvideShippingInfo` interface from `Divergent.ITOps.Interfaces`.  The implementation should use `VolumeEstimator` and `WeightCalculator` to implement the `GetPackageInfo` method.

```c#
public class ShippingInfoProvider : IProvideShippingInfo
{
    public PackageInfo GetPackageInfo(IEnumerable<int> productIds)
    {
        var count = productIds.Count();

        return new PackageInfo
        {
            Weight = WeightCalculator.CalculateWeight(count),
            Volume = VolumeEstimator.Calculate(count)
        };
    }
}
```

### Step 2

Open `ShipWithFedexCommandHandler.cs` in `Divergent.ITOps`. It should now also take a constructor dependency on `IProvideShippingInfo`.

```c#
public class ShipWithFedexCommandHandler : IHandleMessages<ShipWithFedexCommand>
{
    private readonly IProvideShippingInfo _shippingProvider;
    private readonly IProvideCustomerInfo _customerProvider;
    private readonly ILogger<ShipWithFedexCommandHandler> _logger;

    public ShipWithFedexCommandHandler(IProvideShippingInfo shippingProvider, IProvideCustomerInfo customerProvider, ILogger<ShipWithFedexCommandHandler> logger)
    {
        _shippingProvider = shippingProvider;
        _customerProvider = customerProvider;
        _logger = logger;
    }

    public async Task Handle(ShipWithFedexCommand message, IMessageHandlerContext context)
    {
        _logger.LogInformation("Handle ShipWithFedexCommand");

        var shippingInfo = _shippingProvider.GetPackageInfo(message.Products);
        var customerInfo = _customerProvider.GetCustomerInfo(message.CustomerId);

        var fedExRequest = CreateFedexRequest(shippingInfo, customerInfo);
        await CallFedexWebService(fedExRequest);
        _logger.LogInformation("Order {MessageOrderId} shipped with Fedex", message.OrderId);
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

If anything strange happens, try do execute a full rebuild of the entire solution.

## Conclusion

This exercise has demonstrated how to address integration issues where, as with the UI, data from multiple services needs to be combined in order to perform some action. We also saw, again, how powerful sagas can be in orchestrating business processes.

If you'd like to discuss this more, please don't hesitate to drop us a line in our [community discussion forum](https://discuss.particular.net/).
