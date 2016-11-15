# Frequently Asked Questions

This document contains various questions & answers for issues that might arise during the workshop. If the answer is not found, consult your trainer on-site.

## Regular questions

#### Prerequisites checker

Before anything else, run the prerequisites checker. It verifies if you

- Have the proper .NET version installed
- Have Visual Studio 2015 installed
- Have write access to c:\temp\ folder
- Have MSMQ installed
- Have MSDTC installed (Microsoft Distributed Transaction Coordinator)
- Have NServiceBus Performance Counters installed

#### How can I empty the orders list or database?

The solution uses Entity Framework migrations with a seed to insert two orders, so there will always be orders. But if you want to start clean you can always remove the SqlLite datastore file from disk.

**Solution** : Delete the file `c:\temp\divergent.sales.sqlite`

## IT/Ops issues

#### AutoFac not able to instantiate `ShipWithFedexCommandHandler`

In exercise 4 it is explained how an interface defined by IT/Ops should be implemented by other services. To keep the example and 'deployment' simple, a post-build event is used to copy the binary assembly to the IT/Ops folder. Via reflection the assembly is loaded and registered in AutoFac, the used dependency injection container.

Due to various reasons, these files might be missing and the error below might occur for either the `IProvideCustomerInfo` or `IProvideShippingInfo` parameters in the constructor of the `ShipWithFedexCommandHandler` class.

**Solution** : Do a rebuild of the entire solution using `CTRL+SHIFT+B`

Example error:
```
Autofac.Core.DependencyResolutionException: None of the constructors found with 'Autofac.Core.Activators.Reflection.DefaultConstructorFinder' on type 'Divergent.ITOps.Handlers.ShipWithFedexCommandHandler' can be invoked with the available services and parameters:
Cannot resolve parameter 'Divergent.ITOps.Interfaces.IProvideShippingInfo shippingProvider' of constructor 'Void .ctor(Divergent.ITOps.Interfaces.IProvideShippingInfo, Divergent.ITOps.Interfaces.IProvideCustomerInfo)'.
```
