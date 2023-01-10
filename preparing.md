# Preparing your machine for the workshop

- [Install the pre-requisites](#install-the-pre-requisites)
- [Get a copy of this repository](#get-a-copy-of-this-repository)
- [LiteDb Studio](#litedb-studio)
- [Build the exercise solutions](#build-the-exercise-solutions)

NOTE: you can optionally also [prepare your machine in advance for the advanced exercises](#preparing-your-machine-for-the-advanced-exercises).

### Install the pre-requisites

To complete the exercises, you require a Windows machine and Visual Studio or Rider.

Although specific versions of the pre-requisites are mentioned here, later versions may also work. When we confirm that the exercises work with later versions of the pre-requisites, we will update these instructions to refer to those later versions.

#### Visual Studio

Install [Visual Studio 2022](https://www.visualstudio.com)  
Either Community, Professional, or Enterprise with the following workloads:

- .NET desktop development
- ASP.NET and web development

When running the exercises, you will need to run 11 projects at the same time. It is after all a distributed system. To make this easier, configuration was added for the [SwitchStartupProject](https://marketplace.visualstudio.com/items?itemName=vs-publisher-141975.SwitchStartupProjectForVS2022) plugin. More information can be found below.

#### JetBrains Rider

JetBrains Rider 2022.3 is also supported.

### Get a copy of this repository

Clone or download this repo. If you're downloading a zip copy of the repo, ensure the zip file is unblocked before decompressing it:

- Right-click on the downloaded copy
- Click "Properties"
- On the "General" properties page, check the "Unblock" checkbox
- Click "OK"

### LiteDb Studio

The exercises use [LiteDb](https://www.litedb.org/) for storing data.

LiteDB is a serverless database delivered in a single small DLL fully written in .NET C# managed code. It is included in the required projects using NuGet packages. The database files are stored in a `.db` folder in the root of each exercise its solution folder.

Although not required for the exercises, if you want to see what is stored inside each database, you can [download](https://github.com/mbdavid/LiteDB.Studio/releases) and use LiteDb Studio to open each database file individually.

**NOTE**: If you open a database, open it as *'shared'* as otherwise LiteDb Studio will lock the database and your exercises won't work anymore.

### Build the exercise solutions

The exercises are contained in eight Visual Studio solutions under [exercises](exercises). All the solutions require NuGet package restore. This may be possible at the workshop venue (you can verify with the workshop organizers if internet access is available at the venue) but to ensure you can build the solutions during the workshop, we recommend you restore all NuGet packages and build all the solutions before the workshop starts. The simplest way to do this is to open a command prompt, navigate to your copy of this repo, and run `.\build.cmd exercises`. (For a full list of build targets, run `.\build.cmd -T`, or `.\build.cmd -h` for help.)

**You can safely ignore any compiler warnings**.

TIP: For a faster build, specify the `--parallel` (or `-p`) option to build all the exercise solutions in parallel. This can result in a 50% reduction in build time! Bear in mind that the console output from all the solutions will be mixed together, which could make it more difficult to diagnose failures. If the build does fail, it may be best to re-run it without the `--parallel` option before diagnosing the problem.