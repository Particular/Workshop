# Workshop (with examples in NServiceBus)

Welcome to the workshop.



|  |  | **Please ensure you have prepared your machine well in advance of the workshop. Your time during the workshop is valuable, and we want to use it for learning, rather than setting up machines.** | | |
| ---- | :----------------------------------------------------------: | :--: | :--: | ---- |



## Prerequisites

- Windows
- .NET 7.0 or newer
- A .NET IDE for the exercises
  - Visual Studio 2022 ([version 17.4](https://github.com/dotnet/core/blob/main/release-notes/7.0/7.0.0/7.0.0.md#visual-studio-compatibility)) or later
    - [SwitchStartupProject](https://marketplace.visualstudio.com/items?itemName=vs-publisher-141975.SwitchStartupProjectForVS2022)

  - JetBrains Rider 2022.3 or later

- Optional: [Postman](https://www.postman.com/downloads/)  
  <sup>See FAQ for more information.</sup>
  
- Optional: [LiteDb Studio](https://github.com/mbdavid/LiteDB.Studio) ([download](https://github.com/mbdavid/LiteDB.Studio/releases))


### Index

If you have any difficulty preparing your machine, or following this document, please raise an issue in this repository ASAP so that we can resolve the problem before the workshop begins.

- [Preparing your machine for the workshop](preparing.md)
- [Running the exercise solutions](running-exercise.md)  
  As this is a distributed system, there are 11 projects to run simultaneously.
- [Demos](#demos)
- [FAQ](#faq)

## Demos

### ASP.NET Core UI composition

The [`asp-net-core` solution](demos/asp-net-core) demonstrates the use of ASP.NET Core to compose a UI with data from several services.

## FAQ

If the answer to your question is not listed here, consult your on-site trainer.

### How can I clear the orders list?

The simplest method is to delete all databases, which will be automatically created by LiteDb.

The databases are individual files that are located in a `.db` folder under the solution folder. The entire folder can be deleted.

### How can I clear all messages?

The exercises use the LearningTransport and LearningPersistance. Delete the entire `.learningtransport` under the solution folder.

### How can I see what's inside each database?

You can download [LiteDb Studio](https://github.com/mbdavid/LiteDB.Studio/releases) and open each database individually. The database files are stored in a `.db` folder in the root of each exercise its solution folder.

**NOTE**: If you open a database, open it as *'shared'* as otherwise LiteDb Studio will lock the database and your exercises won't work anymore.

### How can I test if the different HTTP API are working?

Sometimes there are issues in the API. Because of the CompositionGateway it might not be directly clear what the problem is.

- Verify if the different API projects are running in Visual Studio and/or JetBrains Rider
  - Divergent.Sales.API
  - Divergent.Finance.API
  - Divergent.Customers.API
  - Divergent.CompositionGateway
- Verify if the response is correct using [Postman](https://www.postman.com/downloads/) be sending HTTP requests to the following uri:
  - http://localhost:20185/api/orders/ for sales
  - http://localhost:20187/api/prices/orders/total?orderIds=1 for finance
  - http://localhost:20186/api/customers/byorders?orderIds=1 for customers
  - http://localhost:4457/orders/ for the Composition Gateway

