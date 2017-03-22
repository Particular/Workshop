# Frequently Asked Questions

This document contains various questions & answers for issues that might arise during the workshop. If the answer is not found, consult your trainer on-site.

## How to get ready for the workshop

Please be sure you have completed the following steps to prepare your machine:

### Get a copy of this repository

Clone or download this repo locally on your machine. If you're downloading a zip copy of the repo please be sure the zip file is unblocked before decompressing it. In order to unblock the zip file:
* Right-click on the downloaded copy
* Choose Property
* On the Property page tick the unblock checkbox
* Press OK

### Run the Particular Platform Installer

In order to correctly install MSMQ and configure the DTC download and run the [Particular Platform Installer](https://particular.net/start-platform-download). At the installation screen select at least:

* `Configure Microsoft Message Queuing`
* `Configure MSDTC for NServiceBus`

All other components are optional.

### Check your machine is correctly configured

In order to run the exercises the following machine configuration is required:

* Powershell execution policy to allow script execution, from an elevated Powershell run the following:
```
Set-ExecutionPolicy Unrestricted
```
* Visual Studio 2015 Update 3 (Community Edition is supported), available for download at https://www.visualstudio.com/downloads/
* .Net framework 4.6.1 Targeting pack for Visual Studio, available for download at https://www.microsoft.com/en-us/download/details.aspx?id=49978
* A SQL Server edition or the `LocalDb` instance installed by Visual Studio, in case of a clean machine with `LocalDb` only please install:
   * Microsoft ODBC Driver 11 for SQL Server, available for download at https://www.microsoft.com/en-us/download/details.aspx?id=36434
   * Microsoft ODBC Command Line Utilities 11 for SQL Server, available for download at https://www.microsoft.com/en-us/download/details.aspx?id=36433

NOTE: On a clean machine do not install latest version, as of this writing 13.1, of Microsoft ODBC Driver and Microsoft ODBC Command Line Utilities as the latter contains a bug that prevents the `LocalDb` instance to be accessible at configuration time.

### Databases setup

To simplify `LocalDB` instance setup 2 PowerShell scripts, in the [exercises/scripts](exercises/scripts) folder, are provided for your convenience. Both need to be run from an elevated PowerShell console.

* Run `Setup.ps1`, with elevation, to create the `LocalDB` instance and all the required databases
* Run `Teardown.ps1`, with elevation, to drop all the databases and delete the `LocalDB` instance

The created `LocalDB` instance is named `(localdb)\microservices-workshop`.

### Nuget packages restore

The exercises are composed of 8 different Visual Studio solutions. All the solutions stored on GitHub rely upon `Nuget package restore` to be run at the first build. Please verify with the workshop organizers if internet access is available at the venue. It is required to run the Nuget restore, otherwise be sure to run the `Nuget package restore` for each solution before attending the workshop.

## Regular questions

#### How can I empty the orders list or database?

Simply connect to the `(localdb)\microservices-workshop` SQL Server instance and manually delete, or truncate, tables that need to be rset. Another option is to run, from an elevated PowerShell console, the `Teardown.ps1` script found in the [exercises/scripts](exercises/scripts) folder. Be aware that the `Teardown.ps1` script will reset the entire instance.

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
