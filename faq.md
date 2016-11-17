# Frequently Asked Questions

This document contains various questions & answers for issues that might arise during the workshop. If the answer is not found, consult your trainer on-site.

## How to get ready for the workshop

If you are attending one of the workshop edition please be sure your machine is ready.

### Get a copy of this repository

Clone or download this repo locally on your machine. If you're downloading a zip copy of the repo please be sure the zip file is unblocked before decompressing it. In order to unblock the zip file:
 * right click on the downloaded copy
 * chose Property
 * on the Property page tick the unblock checkbox
 * press OK

### Check your machine is correctly configured

The cloned, or downloaded, copy of this repository contains an utility ([WorkshopPrerequisitesChecker](https://github.com/Particular/Workshop.Microservices/blob/master/WorkshopPrerequisitesChecker)) that will verify that your machine is configured to run the workshop exercises.

Run the Prerequisites checker, by right clicking on the `WorkshopPrerequisitesChecker.exe` and chosing `Run as Administrator`. It will verify that you:

- Have the proper .NET version installed
- Have Visual Studio 2015 installed
- Have write access to c:\temp\ folder
- Have MSMQ installed, if not it will be installed and properly configured
- Have MSDTC installed (Microsoft Distributed Transaction Coordinator), if not it will be installed and properly configured
- Have NServiceBus Performance Counters installed, if not it will be installed and properly configured

### Nuget packages restore

Exercises are composed by 9 different Visual Studio solutions, all the solutions stored on GitHub relies on the `Nuget package restore` to be run at the first build. Please verify with the workshop organizers if internet access is available at the venue. It is required to run the Nuget restore, otherwise be sure to run the `Nuget package restore` for each solution before attending the workshop.

## Regular questions

#### How can I empty the orders list or database?

Solution uses Entity Framework migrations with a seed to insert two orders, so there will always be orders. But if you want to start clean you can always remove the SqlLite datastore file from disk.

**Solution** : Delete the file `c:\temp\divergent.sales.sqlite`

## Exercise 4 - IT/Ops known issues

#### AutoFac not able to instantiate `ShipWithFedexCommandHandler`

In exercise 4 it is explained how an interface defined by IT/Ops should be implemented by other services. To keep the example and 'deployment' simple, a post-build event is used to copy the binary assembly to the IT/Ops folder. Via reflection the assembly is loaded and registered in AutoFac, the used dependency injection container.

Due to various reasons, these files might be missing and the error below might occur for either the `IProvideCustomerInfo` or `IProvideShippingInfo` parameters in the constructor of the `ShipWithFedexCommandHandler` class.

**Solution** : Do a rebuild of the entire solution using `CTRL+SHIFT+B`

Example error:
```
Autofac.Core.DependencyResolutionException: None of the constructors found with 'Autofac.Core.Activators.Reflection.DefaultConstructorFinder' on type 'Divergent.ITOps.Handlers.ShipWithFedexCommandHandler' can be invoked with the available services and parameters:
Cannot resolve parameter 'Divergent.ITOps.Interfaces.IProvideShippingInfo shippingProvider' of constructor 'Void .ctor(Divergent.ITOps.Interfaces.IProvideShippingInfo, Divergent.ITOps.Interfaces.IProvideCustomerInfo)'.
```
