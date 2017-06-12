# Workshop: Microservice development (with examples in NServiceBus)

- [How to get ready for the workshop](#how-to-get-ready-for-the-workshop)
- [Agenda](#agenda)
- [FAQ](faq.md)

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

## Agenda 
### Day 1
| Time  | Content |
| ------------- | ------------- |
|08:30-09:00| Computer setup time |
|09:00-10:15| Lecture |
|10:15-10:30| Break|
|10:30-11:45| Coding session 1 - UI decomposition|
|11:45-12:30| Lunch|
|12:30-13:30| Summary Lecture Q&A|
|13:30-13:45| Break|
|13:45-15:00| Lecture – new topic|
|15:00-15:15| Break|
|15:15-16:15| Coding session 2 - Publish/subscribe event-processing interactions|
|16:15-16:30| Break|
|16:30-17:15| Summary Lecture Q&A|

### Day 2
| Time  | Content |
| ------------- | ------------- |
|09:00-10:15| Lecture |
|10:15-10:30| Break|
|10:30-11:45| Coding session 3 - Sagas: Long-running multi-stage business processes and policies|
|11:45-12:30| Lunch|
|12:30-13:30| Summary Lecture Q&A|
|13:30-13:45| Break|
|13:45-15:00| Lecture – new topic|
|15:00-15:15| Break|
|15:15-16:15| Coding session 4 - Commands and Reliable integration with 3rd party systems|
|16:15-16:30| Break|
|16:30-17:15| Summary Lecture Q&A|
