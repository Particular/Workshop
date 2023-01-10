# Workshop (with examples in NServiceBus)

Welcome to the workshop.



|  |  | **Please ensure you have prepared your machine well in advance of the workshop. Your time during the workshop is valuable, and we want to use it for learning, rather than setting up machines.** | | |
| ---- | :----------------------------------------------------------: | :--: | :--: | ---- |



## Prerequisites

- Windows
- .NET 7.0 or newer
- A .NET IDE for the exercises
  - Visual Studio 2022 or later
    - [SwitchStartupProject](https://marketplace.visualstudio.com/items?itemName=vs-publisher-141975.SwitchStartupProjectForVS2022)

  - JetBrains Rider 2022.3 or later


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
