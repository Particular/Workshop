# Workshop (with examples in NServiceBus)

**Please ensure you have prepared your machine well in advance of the workshop. Your time during the workshop is valuable, and we want to use it for learning, rather than setting up machines.**

If you have any difficulty preparing your machine, or following this document, please raise an issue in this repository ASAP so that we can resolve the problem before the workshop begins.

- [Preparing your machine for the workshop](#preparing-your-machine-for-the-workshop)
- [Running the exercise solutions](#running-the-exercise-solutions)
- [Preparing your machine for the advanced exercises](#preparing-your-machine-for-the-advanced-exercises)
- [Demos](#demos)
- [FAQ](#faq)

## Preparing your machine for the workshop

- [Install the pre-requisites](#install-the-pre-requisites)
- [Get a copy of this repository](#get-a-copy-of-this-repository)
- [Set up the databases](#set-up-the-databases)
- [Build the exercise solutions](#build-the-exercise-solutions)

### Install the pre-requisites

To complete the exercises, you require a Windows machine and Visual Studio. You must be using a Windows client edition, such as Windows 10, rather than a server edition, such as Windows Server 2016. The Particular Platform Installer does not support server editions of Windows.

#### Visual Studio

Install [Visual Studio 2017](https://www.visualstudio.com) ver. 15.3 and higher (Community, Professional, or Enterprise) with the following workloads:
  - .NET desktop development
  - ASP.NET and web development
  - .NET Core cross-platform development

If you are not running the latest version of Visual Studio 2017, you may need to install the .NET Core SDK seperately. Install [version 2.0.3 or newer](https://www.microsoft.com/net/download/all).

#### MSMQ and DTC

To ensure MSMQ and DTC are correctly installed, run the [Particular Platform Installer](https://particular.net/start-platform-download).

NOTE: If you are using Microsoft Edge, see [the FAQ](#how-do-i-download-the-particular-platform-installer-with-microsoft-edge).

In the installation screen, select a minimum of:

* "Configure MSDTC for NServiceBus"
* "Configure Microsoft Message Queuing"

All other components are optional.

### Get a copy of this repository

Clone or download this repo. If you're downloading a zip copy of the repo, ensure the zip file is unblocked before decompressing it:

* Right-click on the downloaded copy
* Click "Properties"
* On the "General" properties page, check the "Unblock" checkbox
* Click "OK"

### Set up the databases

WARNING: There is a bug in SQL Server 2017 LocalDB which results in incorrect paths being used for database files. If you have issues running the SQL script, uninstall SQL Server 2017 LocalDB. You should then be able to use SQL Server 2016 LocalDB, which is installed with Visual Studio 2017.

Open an **elevated** command prompt, navigate to your copy of this repo, and run:

```Batchfile
powershell -NoProfile -ExecutionPolicy unrestricted -File exercises\scripts\Setup-LocalDBInstance.ps1
```

When you no longer need to run the exercises, you may optionally run `Teardown-LocalDBInstance.ps1`.

Now connect to your LocalDB instance and run `exercises\scripts\Setup-Databases.sql`.

You can do this using either SQL Server Management Studio (if you already have it installed) or Visual Studio. If using Visual Studio:

- Open `exercises\scripts\Setup-Databases.sql`
- From the Visual Studio menus, select SQL -> Execute
- Choose this instance: Local -> microservices-workshop
- Click "Connect"
- After the query has run, ensure that you see "Command(s) completed successfully."

### Build the exercise solutions

The exercises are contained in eight Visual Studio solutions under [exercises](exercises). All the solutions require NuGet package restore. This may be possible at the workshop venue (you can verify with the workshop organizers if internet access is available at the venue) but to ensure you can build the solutions during the workshop, we recommend you restore all NuGet packages and build all the solutions before the workshop starts. The simplest way to do this is to open a command prompt, navigate to your copy of this repo, and run `.\build.cmd exercises`. (For a full list of build targets, run `.\build.cmd -T`, or `.\build.cmd -h` for help.) You can safely ignore any compiler warnings.

## Running the exercise solutions

Before opening any exercise solutions, set the startup projects by navigating to your copy of this repo and running `set-startup-projects.cmd`. Note that if you `git clean` your clone, you will have to run this command again.

The startup projects are also listed in the instructions for each exercise. If you need to, you can configure them manually:

  - In Visual Studio, right click the solution in the Solution Explorer
  - Click "Properties"
  - Ensure that, in the left hand pane, "Common Properties", "Start Project" is selected.
  - Select the "Multiple startup projects" radio button
  - Set the "Action" for each project listed in the instructions for the exercise to "Start".

To run an exercise solution, simply press <kbd>F5</kbd> in Visual Studio. The exercise solution will now be running and fully functional.

### Note

The solutions contain single page applications (SPAs) and use `IIS Express`. To prevent caching issues, before switching to another exercise:
  - Ensure that `IIS Express` is shut down
  - Clear the browser cache (or disable it entirely). Alternatively, the cache can cleared by refreshing the page using <kbd>Shift</kbd>+<kbd>F5</kbd> or <kbd>Ctrl</kbd>+<kbd>Shift</kbd>+<kbd>R</kbd> in some browsers.
- When running a solution, the wrong page is sometimes displayed in the browser. Either:
  - Ensure all HTML template files are closed when the application is run, or:
  - Manually change the browser address to the root URL.

## Preparing your machine for the advanced exercises

If you completed all the standard exercises, first of all, well done!

You can now attempt the advanced exercises. Don't worry if you don't manage to start or complete the advanced exercises during the workshop. You can also attempt them after the workshop is finished. If you have questions, you can ask them during the workshop or in our [community discussion forum](https://discuss.particular.net/).

It is likely that you will have to read documentation for the Particular Service Platform to finish the advanced exercises. Links to documentation will be provided.

To complete the advanced exercises, you first need to install the rest of the Particular Service Platform:

- ServicePulse: a web app which monitors your endpoints.
- ServiceInsight: a desktop app which visualises the flow of messages within and between endpoints in detail.
- ServiceControl: a Windows service which collects the data that drives ServicePulse and ServiceInsight.

Run the [Particular Platform Installer](https://particular.net/start-platform-download) again, selecting ServiceControl, ServiceInsight, and ServicePulse.

When installation has completed, click "Start ServiceControl Management".

You may now close the Particular Service Platform Installation window.

In the ServiceControl Management window, click "Add new instance".

In the settings for the new instance, there are two choices you must make:

- "Transport": choose "MSMQ".
- "Audit forwarding": choose "Off".

All other settings should be left with their default values.

Click "Add".

After the instance has been added, make a note of the name of the instance. This will usually be "Particular.ServiceControl".

You may now close the ServiceControl Management window.

You're now ready to attempt the advanced exercises!

## Demos

### ASP.NET Core UI composition

The [`asp-net-core` solution](demos/asp-net-core) demonstrates the use of ASP.NET Core to compose a UI with data from several services.

### ServicePulse monitoring & recovery

The [self contained platform demo](https://docs.particular.net/tutorials/monitoring-demo/) includes ServicePulse, ServiceControl, ServiceControl Monitoring and a few endpoints to demonstrate monitoring and recovery.

## FAQ

If the answer to your question is not listed here, consult your on-site trainer.

### How can I clear the orders list?

The simplest method is to reset all the databases (see below). Bear in mind that this will reset _all the databases_ for _all the exercises_.

Alternatively, if you want to clear the orders list for a specific exercise, connect to the LocalDB instance, and either pick out and run the `drop` and `create` statements for that exercise from the scripts, or manually delete, or truncate, the tables that need to be cleared.

To reset all the databases:

Connect to the instance and run `Teardown-Databases.sql` followed by `Setup-Databases.sql`, both located in the [exercises/scripts](exercises/scripts) folder.

### Can I use Windows 7?

Yes. However, Visual Studio 2017 comes with SQL Server 2016 LocalDB, which is not compatible with Windows 7. Instead, you will need to install [SQL Server 2014 LocalDB](https://www.microsoft.com/en-us/download/details.aspx?id=42299). When prompted to choose which file to download, choose either the 64-bit or 32-bit version of `SqlLocalDB.msi`, depending on [your Windows installation](https://support.microsoft.com/en-gb/help/15056/windows-7-32-64-bit-faq).

### How do I download the Particular Platform Installer with Microsoft Edge?

When you attempt to download the installer, you may be presented with this message:

![download prompt](img/platform-installer-on-edge/download-prompt.png?raw=true)

If so, click on "View Downloads" to show the "Downloads" window:

![download list](img/platform-installer-on-edge/downloads-list.png?raw=true)

**Right-click** the red text "This unsafe download was blocked by SmartScreen Filter." to show the context menu:

![download unsafe file](img/platform-installer-on-edge/download-unsafe-file-option.png?raw=true)

 Click "Download unsafe file".
