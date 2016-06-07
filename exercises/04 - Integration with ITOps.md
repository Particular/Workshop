# Exercise 04 - Integration

You will quickly run into the need for integration towards 3rd party partners through Web Services, REST based APIs, sending emails or file exports. Most of the time they will need to mash up data from several services before interacting with the 3rd party, similar to a composite UI.

Use a specific endpoint called ITOps for integration. It defines a set of interfaces for getting the data it needs to do an integration, and the services implements these. We call them providers.  Each service needs to provide the functionality for accessing its data to avoid breaking service autonomy. Each service deploys its providers into the ITOps endpoint, which will co-host all of them and perform the third party integration. It is important to note that ITOps has no dependencies on any service. It only defines provider interfaces and uses whatever implementations your services provide for them.

## Introduction
In the last exercise, we created a saga in Shipping to wait for `OrderSubmittedEvent` and `PaymentSucceededEvent`. We now want the saga to send a `ShipWithFedexCommand` to ITOps so that FedEx will pick the package up.  ITOps needs to respond to that message by calling implementations of its `IProvideCustomerInfo` and `IProvideShippingInfo` interfaces. The customers service owns customer data, so it will need to implement `IProvideCustomerInfo`. Shipping holds the information regarding the weight and volume of the package, so it provides an implementation of `IProvideShippingInfo`.


A note about deployment: In a production environment one would package each provider into a deployable, like a NuGet package, and deploy them to the ITOPs endpoint. For this exercise, we will use a post-build event in Customers.Data and Shipping.Data to copy the providers into ITOPs for simplicity.

### Business requirements

Once an order is ready to ship, we want to initiate shipping by telling our shipping partner to come pick the package up. We do this by calling their Web Service with the customer name, address, weight and volume of the package.

## Exercise 04.1 - Send and handle integration command from Shipping to ITops

In this exercise, we'll have the saga in `Shipping` tell ITOps to integrate with FedEx. The saga will do this by sending a command to ITOps when it knows an order has both been placed and paid for. ItOps will fetch the data through providers and call FedEx.

**1)** Compile the application to retrieve all NuGet packages.

**2)** Open the `Divergent.ItOPs.Messages` project.

**3)** Create a new class `ShipWithFedexCommand` in the commands folder. It should contain the order id, customer id and a list of the product ids.
```
public class ShipWithFedexCommand
{
    public Guid OrderId { get; set; }
    public Guid CustomerId { get; set; }
    public List<Guid> Products { get; set; }
}
```

**4)** Open the `Divergent.Shipping` project.

**5)** Open `ShippingSaga.cs` and look at the `ProcessOrder` method. The method should check if the order has been both submitted (`Data.IsOrderSubmitted`) and paid for (`Data.IsPaymentProcessedYet`). If so, it should send the new `ShipWithFedexCommand`.

**6)** Open app.config in the `Divergent.Shipping` project. 

**7)** Add a new message mapping to let `Shipping` know where to send messages for ITOPs. Do this by adding `<add Assembly="Divergent.ITOps.Messages" Endpoint="Divergent.ITOps" />` to the UnicastBus/MessageEndpointMappings element. It should look like this afterwards: 
```
  <UnicastBusConfig>
    <MessageEndpointMappings>
      <add Assembly="Divergent.Finance.Messages" Endpoint="Divergent.Finance" />
      <add Assembly="Divergent.Sales.Messages" Endpoint="Divergent.Sales" />
      <add Assembly="Divergent.ITOps.Messages" Endpoint="Divergent.ITOps" />
    </MessageEndpointMappings>
  </UnicastBusConfig>
```

**8)** Open the `Divergent.ITops` project.

**9)** Add a class under Handlers called `ShipWithFedexCommandHandler`. It should contain an message handler for `ShipWithFedexCommand` that calls an imaginary FedEx Web Service. Hardcode the customer information for now. Like this:
```
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
            new XElement("FedExShipment",
                new XElement("ShipTo",
                    new XElement("Name", name),
                    new XElement("Street", street),
                    new XElement("City", city),
                    new XElement("PostalCode", postalCode),
                    new XElement("Country", country)));                    
        return shipment.Document;
    }

    private Task CallFedexWebService(XDocument fedExRequest)
    {
        //do web service call etc.         
        return Task.FromResult(0);
    }
}
```

**10)** Run and test that the ITOps message handler is invoked whenever you submit a new order in the UI.


## Exercise 04.2 - Implement customer provider

In this exercise, we'll implement the customer provider in the Customer Service and make ITOps uses it to fetch the customer information when it does the FedEx integration.

**1)** Open the `Divergent.ITops.Interfaces` project. It contains an interface called `IProvideCustomerInfo.cs`. It has a single method that takes a customer id and returns a `CustomerInfo` instance with name, street, etc. for the specified customer.

**2)** Open the `Divergent.Customers.Data` project. Add a new class called `CustomerInfoProvider` in the ITOps folder. It should implement the `IProvideCustomerInfo` interface from `Divergent.ITOps.Interfaces`.  The implementation should instantiate an instance of CustomerRepository to implement the `GetCustomerInfo` method. Like this:
```
public class CustomerInfoProvider : IProvideCustomerInfo
{
    private readonly ICustomerRepository _repository = new CustomerRepository();

    public async Task<CustomerInfo> GetCustomerInfo(Guid customerId)
    {
        var customer = await _repository.Customer(customerId);
        
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

**3)** Go to properties on the `Divergent.Customers.Data` project. Double check that it contains a post-build event with `copy /Y "$(TargetDir)$(ProjectName).dll" "$(SolutionDir)Divergent.ITOps\Providers\$(ProjectName).dll"`. This "deploys" the provider into a location that ITOps know about to avoid  ITOps referencing any other service directly.

**4)** Open up the `ShipWithFedexCommandHandler.cs` we created in exercise 04.1. We now want this to class to take a constructor dependency on `IProvideCustomerInfo`. It is filled by dependency injection as long as the provider implementations are copied to a location ITOps knows about. The handler should use this dependency to fetch the customer information instead of using the hardcoded values from exercise 04.1. Like this:
```
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
            new XElement("FedExShipment",
                new XElement("ShipTo",
                    new XElement("Name", customerInfo.Name),
                    new XElement("Street", customerInfo.Street),
                    new XElement("City", customerInfo.City),
                    new XElement("PostalCode", customerInfo.PostalCode),
                    new XElement("Country", customerInfo.Country)));
               
        return shipment.Document;
    }

    private Task CallFedexWebService(XDocument fedExRequest)
    {
        //do web service call etc.   
       return Task.FromResult(0);
    }
}
``` 

**5)** Run and test that the ITOps message handler now fetches the customer information with the customer provider.

Check out `ReflectionHelper` and `ContainerSetup` in `Divergent.ITOps` to learn more about how ITOPs loads and co-hosts the providers.


## Exercise 04.3 - Implement shipping provider 

In this exercise, we'll implement the shipping provider in the Shipping service, pretty much similar to what we did in exercise 04.2. ITops defines an `IProvideShippingInfo` interface. The provider must be implemented in the Shipping service using the two helpers `VolumeEstimator` and `WeightCalculator` already present there.

**1)** Open the `Divergent.Shipping.Data` project. Add a new class called `ShippingInfoProvider` in the ITOps folder. It should implement the `IProvideShippingInfo` interface from `Divergent.ITOps.Interfaces`.  The implementation should use `VolumeEstimator` and `WeightCalculator` to implement the `GetPackageInfo` method. Like this:
```
public class ShippingInfoProvider : IProvideShippingInfo
{
    public Task<PackageInfo> GetPackageInfo(List<Guid> productIds)
    {
        return Task.FromResult(new PackageInfo()
        {
            Weight = WeightCalculator.CalculateWeight(productIds.Count),
            Volume = VolumeEstimator.Calculate(productIds.Count)
        });
    }
}
```

**2)** Go to properties on the `Divergent.Shipping.Data` project. Double check that it contains a post-build event with `copy /Y "$(TargetDir)$(ProjectName).dll" "$(SolutionDir)Divergent.ITOps\Providers\$(ProjectName).dll"`. This "deploys" the provider into a location that ITOps know about, avoiding the need for ITOps to reference any other service directly.

**3)** Open up `ShipWithFedexCommandHandler.cs` in `Divergent.ItOps`. This should now also take a constructor dependency on `IProvideShippingInfo`. This will be filled by dependency injection. Update the handler to use this new dependency to fetch the shipping information. Like this:
```
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
            new XElement("FedExShipment",
                new XElement("ShipTo",
                    new XElement("Name", customerInfo.Name),
                    new XElement("Street", customerInfo.Street),
                    new XElement("City", customerInfo.City),
                    new XElement("PostalCode", customerInfo.PostalCode),
                    new XElement("Country", customerInfo.Country)),
                new XElement("Measurements",
                    new XElement("Volume", packageInfo.Volume),
                    new XElement("Weight", packageInfo.Weight)));
        return shipment.Document;
    }

    private Task CallFedexWebService(XDocument fedExRequest)
    {
        //do web service call etc.         
        return Task.FromResult(0);
    }
}
```
**4)** Run and test that the ITOps message handler now fetches both the customer and shipping information from the providers.

### Solution configuration

#### Start-up projects

* Divergent.Customers
* Divergent.Customers.API
* Divergent.Finance
* Divergent.Finance.API
* Divergent.Frontend.SPA
* Divergent.ITOps
* Divergent.Sales
* Divergent.Sales.API
* Divergent.Shipping
* PaymentProviders
