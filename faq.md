# Frequently Asked Questions

This document contains various questions & answers for issues that might arise during the workshop. If the answer is not found, consult your trainer on-site.

- [How can I empty the orders list or database?](#how-can-I-empty-the-orders-list-or-database)
- [Why does Autofac throw an exception when running exercise 4?](#why-does-autofac-throw-an-exception-when-running-exercise-4)

## How can I empty the orders list or database?

Simply connect to the `(localdb)\microservices-workshop` SQL Server instance and manually delete, or truncate, tables that need to be rset. Another option is to run, from an elevated PowerShell console, the `Teardown.ps1` script found in the [exercises/scripts](exercises/scripts) folder. Be aware that the `Teardown.ps1` script will reset the entire instance.

## Why does Autofac throw an exception when running exercise 4?

In exercise 4 it is explained how an interface defined by IT/Ops should be implemented by other services. To keep the example and 'deployment' simple, a post-build event is used to copy the binary assembly to the IT/Ops folder. Via reflection the assembly is loaded and registered in AutoFac, the used dependency injection container.

Due to various reasons, these files might be missing and the error below might occur for either the `IProvideCustomerInfo` or `IProvideShippingInfo` parameters in the constructor of the `ShipWithFedexCommandHandler` class.

**Solution** : Do a rebuild of the entire solution using `CTRL+SHIFT+B`

Example error:

```
Autofac.Core.DependencyResolutionException: None of the constructors found with 'Autofac.Core.Activators.Reflection.DefaultConstructorFinder' on type 'Divergent.ITOps.Handlers.ShipWithFedexCommandHandler' can be invoked with the available services and parameters:
Cannot resolve parameter 'Divergent.ITOps.Interfaces.IProvideShippingInfo shippingProvider' of constructor 'Void .ctor(Divergent.ITOps.Interfaces.IProvideShippingInfo, Divergent.ITOps.Interfaces.IProvideCustomerInfo)'.
```
