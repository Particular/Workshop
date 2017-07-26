# Microservices workshop (with examples in NServiceBus)

**Please ensure you have prepared your machine well in advance of the workshop. Your time during the workshop is valuable, and we want to use it for learning, rather than setting up machines.**

If you have any difficulty preparing your machine, or following this document, please raise an issue in this repository ASAP so that we can resolve the problem before the workshop begins.

- [Preparing your machine for the workshop](#preparing-your-machine-for-the-workshop)
- [Running the exercise solutions](#running-the-exercise-solutions)
- [FAQ](#faq)

## Preparing your machine for the workshop

### Install the pre-requisites

To complete the exercises, you require a Windows machine and Visual Studio. Please ensure you have followed these steps:

* Install [Visual Studio 2017](https://www.visualstudio.com/downloads/) or Visual Studio 2015 Update 3 (the "Community" edition is enough).

* If you have a full SQL Server instance installed, you can choose to use that for the exercises. Otherwise, you will be using LocalDB. In the case of a clean machine with LocalDB only, please install:
  * [Microsoft ODBC Driver 11 for SQL Server](https://www.microsoft.com/en-us/download/details.aspx?id=36434)
  * [Microsoft ODBC Command Line Utilities 11 for SQL Server](https://www.microsoft.com/en-us/download/details.aspx?id=36433)

  NOTE: Do not install versions 13.1 of these components as they contain a bug that prevents the LocalDB instance from being accessible at configuration time.

* Set the PowerShell execution policy to allow script execution. From an elevated PowerShell prompt, run the following:
  ```PowerShell
  Set-ExecutionPolicy Unrestricted
  ```

### Get a copy of this repository

Clone or download this repo to your local machine.

If you're downloading a zip copy of the repo, please ensure the zip file is unblocked before decompressing it:

* Right-click on the downloaded copy
* Click "Properties"
* On the "General" properties page, check the "Unblock" checkbox
* Click "OK"

### Run the Particular Platform Installer

To ensure MSMQ and the DTC are correctly installed, please run the [Particular Platform Installer](https://particular.net/start-platform-download). In the installation screen, select a minimum of:

* "Configure MSDTC for NServiceBus"
* "Configure Microsoft Message Queuing"

All other components are optional.

### Set up the databases

#### When using LocalDB

The simplest way to manage the databases is to use the PowerShell scripts located in [exercises/scripts](exercises/scripts).

**These scripts must be run from an elevated PowerShell prompt.**

* Run `Setup-Databases.ps1` to create a LocalDB instance named `(localdb)\microservices-workshop`, containing all the required databases.

When you have finished the workshop, you may optionally run `Teardown-Databases.ps1` to drop all the databases and delete the LocalDB instance.

NOTE: If you receive errors regarding "Microsoft ODBC Driver", you can work around these by connecting to the `(localdb)\microservices-workshop` database using, for example, Visual Studio or SQL Managerment Studio, and running the SQL contained in the `.sql` file (`Setup-Databases.sql` or `Teardown-Databases.sql`) corresponding to the `.ps1` file which raised the error.

NOTE: If the setup script fails with a "sqllocaldb command not found" error, your machine may be missing some files related to LocalDB. To fix this, try installing [the LocalDB standalone package](https://www.microsoft.com/en-us/download/details.aspx?id=29062) and then re-run the script.

#### When using a full SQL Server instance

Connect to the instance and run `exercises\scripts\Setup-Databases.sql`.

When you have finished the workshop, you may optionally run `Teardown-Databases.sql` to drop all the databases.

### Restore the NuGet packages

The exercises are contained in eight Visual Studio solutions under [exercises](exercises). All the solutions require NuGet package restoration before building. This may be possible at the workshop venue (you can verify with the workshop organizers if internet access is available at the venue) but to ensure you can build the solutions during the workshop, we recommend you run NuGet package restore before the workshop. The simplest way to do this is to open each of the solutions and build them, which will automatically restore the NuGet packages.

## Running the exercise solutions

- Each exercise solution is configured to run and be fully functional just by pressing <kbd>F5</kbd> in Visual Studio.
  - In case you have problems, the projects that need to be configured as startup projects are listed in the instructions for each exercise.
- The solutions contain single page applications (SPAs) and use `IIS Express`. To prevent caching issues, before switching to another exercise:
  - Ensure that `IIS Express` is shut down
  - Clear the browser cache (or disable it entirely). Alternatively, the cache can cleared by refreshing the page using <kbd>Shift</kbd>+<kbd>F5</kbd> or <kbd>Ctrl</kbd>+<kbd>Shift</kbd>+<kbd>R</kbd> in some browsers.
- When running a solution, the wrong page is sometimes displayed in the browser. Either:
  - Ensure all HTML template files are closed when the application is run, or:
  - Manually change the browser address to the root URL.

## FAQ

If the answer to your question is not listed here, consult your on-site trainer.

### How can I clear the orders list?

The simplest method is to reset all the databases (see below). Bear in mind that this will reset _all the databases_ for _all the exercises_.

Alternatively, if you want to clear the orders list for a specific exercise, connect to the SQL Server instance, and either pick out and run the `drop` and `create` statements for that exercise from the scripts, or manually delete, or truncate, the tables that need to be cleared.

To reset all the databases:

#### When using LocalDB

Using an elevated PowerShell prompt, run `Teardown-Databases.ps1` followed by `Setup-Databases.ps1`, both located in the [exercises/scripts](exercises/scripts) folder. 

#### When using a full SQL Server instance (or LocalDB)

Connect to the instance and run `Teardown-Databases.sql` followed by `Setup-Databases.sql`, both located in the [exercises/scripts](exercises/scripts) folder. 
