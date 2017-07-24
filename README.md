# Workshop: Microservice development (with examples in NServiceBus)

- [How to get ready for the workshop](#how-to-get-ready-for-the-workshop)
- [How to run the exercise solutions](#how-to-run-the-exercise-solutions)
- [FAQ](#faq)

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

* Run `Setup-Databases.ps1`, with elevation, to create the `LocalDB` instance and all the required databases
* Run `Teardown-Databases.ps1`, with elevation, to drop all the databases and delete the `LocalDB` instance

The created `LocalDB` instance is named `(localdb)\microservices-workshop`.

NOTE: If you receive errors regarding "Microsoft ODBC Driver", you can work around these by connecting to the `(localdb)\microservices-workshop` database using, for example, Visual Studio or SQL Managerment Studio, and running the SQL contained in the `.sql` file (`Setup-Databases.sql` or `Teardown-Databases.sql`) corresponding to the `.ps1` file which raised the error.

NOTE: In case the database setup script fails with a "sqllocaldb command not found" error it is possible to install LocalDb as a standalone package by downloading it separately at https://www.microsoft.com/en-us/download/details.aspx?id=29062

### Nuget packages restore

The exercises are composed of 8 different Visual Studio solutions. All the solutions stored on GitHub rely upon `Nuget package restore` to be run at the first build. Please verify with the workshop organizers if internet access is available at the venue. It is required to run the Nuget restore, otherwise be sure to run the `Nuget package restore` for each solution before attending the workshop.

## How to run the exercise solutions

- Each exercise solution is configured to be running and fully functional just by pressing <kbd>F5</kbd> in Visual Studio.
  - In case you have problems, the projects that need to be configured as startup projects are listed in the instructions for each exercise.
- The solutions contain single page applications (SPAs) and use `IIS Express`. To prevent caching issues, before switching to another exercise:
  - Ensure that `IIS Express` is shut down
  - Clear the browser cache (or disable it entirely). Alternatively, the cache can cleared by refreshing the page using <kbd>Ctrl</kbd>+<kbd>F5</kbd> in some browsers.
- When running a solution, the wrong page is sometimes displayed in the browser. Either:
  - Ensure all HTML template files are closed when the application is run, or:
  - Manually change the browser address to the root URL.

## FAQ

This section contains various questions & answers for issues that might arise during the workshop. If the answer is not found, consult your trainer on-site.

### How can I empty the orders list or database?

Simply connect to the `(localdb)\microservices-workshop` SQL Server instance and manually delete, or truncate, tables that need to be rset. Another option is to run, from an elevated PowerShell console, the `Teardown.ps1` script found in the [exercises/scripts](exercises/scripts) folder. Be aware that the `Teardown.ps1` script will reset the entire instance.
