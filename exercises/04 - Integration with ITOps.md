# Exercise 04 - Integration

You will quickly run into the need for integration with 3rd party systems. You will use Web Services, REST based APIs, you'll emails or export files. Most of the time data need to be retrieved from several different services before interacting with a 3rd party.

Use a specific endpoint called IT/Ops for integration. It defines a set of interfaces for getting the data needed to integrate with 3rd parties. Services will implement those IT/Ops interfaces. Each service needs to provide the functionality for accessing its own data to avoid breaking service autonomy. Each service deploys its providers to the IT/Ops endpoint, which will co-host all of them and perform the 3rd party integration. It is important to note that IT/Ops has no dependencies on any service. It only defines provider interfaces and uses implementations from services after deployment at run time.

## Introduction
In the last exercise, we created a saga in Shipping to wait for `OrderSubmittedEvent` and `PaymentSucceededEvent`. We now want the saga to send a `ShipWithFedexCommand` to IT/Ops so that FedEx will pick up and ship the package.

IT/Ops needs to respond to that message by calling implementations of its `IProvideCustomerInfo` and `IProvideShippingInfo` interfaces. Customers service owns customer data, so it will provide an implementation of `IProvideCustomerInfo`. Shipping service holds information regarding weight and volume of packages, so it will provide an implementation of `IProvideShippingInfo`.

A note about deployment: In a production environment one would package each provider into a deployable artifact (e.g. a NuGet package) and deploy them to the IT/Ops endpoint. In this exercise for simplicity we will use post-build events in `Divergent.Customers.Data` and `Divergent.Shipping.Data` to copy providers implementations into IT/Ops.

### Business requirements

Once an order is ready to be shipped, we want to initiate the shipping process by telling our shipping partner to pick up the package. We do this by calling their Web Service with the customer name, address, weight and volume of the package to ship.

## Exercise 04.1 - Send and handle integration command from Shipping to IT/Ops

In this exercise, we'll have the saga in `Divergent.Shipping` service tell IT/Ops to integrate with FedEx. The saga will do this by sending a command to IT/Ops when it knows an order has been placed and payment received. IT/Ops will fetch data through providers and call FedEx.

**1)** Compile the application to restore all NuGet packages.

**2)** Open the `Divergent.ItOps.Messages` project.

**3)** Create a new class `ShipWithFedexCommand` in the Commands folder. It should contain the order Id, customer Id, and a list of product Ids.
```
    public class ShipWithFedexCommand
    {
        public int OrderId { get; set; }
        public int CustomerId { get; set; }
        public List<int> Products { get; set; }
    }
```

**4)** Open the `Divergent.Shipping` project.

**5)** Open `ShippingSaga.cs` and look at the `ProcessOrder` method. The method should check if the order has been both submitted (`Data.IsOrderSubmitted`) and paid for (`Data.IsPaymentProcessedYet`). If so, it should send the new `ShipWithFedexCommand`.

```
private async Task ProcessOrder(IMessageHandlerContext context)
{
    if (Data.IsOrderSubmitted && Data.IsPaymentProcessedYet)
    {
        await context.Send<ShipWithFedexCommand>(cmd =>
        {
            cmd.OrderId = Data.OrderId;
            cmd.CustomerId = Data.CustomerId;
            cmd.Products = Data.Products.Select(s => s.Identifier).ToList();
        });

        MarkAsComplete();
    }
}
```

**6)** Open `app.config` in the `Divergent.Shipping` project.

**7)** Add a new message mapping to let `Divergent.Shipping` know where to send messages for IT/Ops. Do this by adding `<add Assembly="Divergent.ITOps.Messages" Endpoint="Divergent.ITOps" />` to the UnicastBus/MessageEndpointMappings element. It should look like this:
```
    <UnicastBusConfig>
      <MessageEndpointMappings>
        <add Assembly="Divergent.Finance.Messages" Endpoint="Divergent.Finance" />
        <add Assembly="Divergent.Sales.Messages" Endpoint="Divergent.Sales" />
        <add Assembly="Divergent.ITOps.Messages" Endpoint="Divergent.ITOps" />
      </MessageEndpointMappings>
    </UnicastBusConfig>
```

**8)** Open the `Divergent.ITOps` project.

**9)** Add a class under Handlers called `ShipWithFedexCommandHandler`. It should contain a message handler for `ShipWithFedexCommand` that calls a fake FedEx Web Service. Hard-code the customer information for now.

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

**10)** Run the solution and verify that the IT/Ops message handler is invoked whenever you submit a new order in the UI.


## Exercise 04.2 - Implement customer provider

In this exercise, we'll implement the customer provider in the Customers service and make IT/Ops use it to fetch the customer information.

**1)** Open the `Divergent.ITOps.Interfaces` project. It contains an interface called `IProvideCustomerInfo.cs`. It has a single method that takes a customer id and returns a `CustomerInfo` instance with name, street, etc. for the specified customer.

**2)** Open the `Divergent.Customers.Data` project. Add a new class called `CustomerInfoProvider` in the ITOps folder. It should implement the `IProvideCustomerInfo` interface from `Divergent.ITOps.Interfaces`. 

```
public class CustomerInfoProvider : IProvideCustomerInfo
{
	public Task<CustomerInfo> GetCustomerInfo(int customerId)
	{
		using (var db = new Context.CustomersContext())
		{
			var customer = db.Customers.Where(c => c.Id == customerId).Single();

			return Task.FromResult(new CustomerInfo
			{
             	Name = customer.Name,
             	Street = customer.Street,
             	City = customer.City,
             	PostalCode = customer.PostalCode,
             	Country = customer.Country,
			});
		}
	}
}
```

**3)** Go to properties on the `Divergent.Customers.Data` project. Double check that it contains a post-build event:

 `copy /Y "$(TargetDir)$(ProjectName).dll" "$(SolutionDir)Divergent.ITOps\Providers\$(ProjectName).dll"`

This "deploys" the provider into a location that IT/Ops knows. IT/Ops doesn't need a direct reference to other services.

**4)** Open up the `ShipWithFedexCommandHandler.cs`. This class should take a constructor dependency on `IProvideCustomerInfo`. The Dependency Injection framework will find the provider implementations as long as they are copied to IT/Ops. The handler should use this dependency to fetch the customer information instead of using the hard-coded values.
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

**5)** Run the solution and verify that the ITOps message handler fetches the customer information using the supplied provider.

NOTE: For your convenience IT/Ops is already configured with the required connection strings to allow providers to function properly.

Check out `ReflectionHelper` and `ContainerSetup` in `Divergent.ITOps` to learn more about how IT/Ops loads and co-hosts the providers.


## Exercise 04.3 - Implement shipping provider

In this exercise, you'll implement the shipping provider in the Shipping service. ITOps defines an `IProvideShippingInfo` interface. The provider will be implemented in the Shipping service using the two helpers `VolumeEstimator` and `WeightCalculator` already present there.

**1)** Open the `Divergent.Shipping.Data` project. Add a new class called `ShippingInfoProvider` in the ITOps folder. It should implement the `IProvideShippingInfo` interface from `Divergent.ITOps.Interfaces`.  The implementation should use `VolumeEstimator` and `WeightCalculator` to implement the `GetPackageInfo` method.

```
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

**2)** Go to the `Divergent.Shipping.Data` project properties. Double check that it contains a post-build event:

`copy /Y "$(TargetDir)$(ProjectName).dll" "$(SolutionDir)Divergent.ITOps\Providers\$(ProjectName).dll"`

**3)** Open up `ShipWithFedexCommandHandler.cs` in `Divergent.ITOps`. it should now also take a constructor dependency on `IProvideShippingInfo`.
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
          return Task.FromResult(0);
      }
  }
```
**4)** Run the solution and verify that the IT/Ops message handler now fetches both the customer and shipping information via the providers.

### Solution configuration

#### Start-up projects

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
