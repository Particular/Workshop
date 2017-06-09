# Frequently Asked Questions

This document contains various questions & answers for issues that might arise during the workshop. If the answer is not found, consult your trainer on-site.

## How can I empty the orders list or database?

Simply connect to the `(localdb)\microservices-workshop` SQL Server instance and manually delete, or truncate, tables that need to be rset. Another option is to run, from an elevated PowerShell console, the `Teardown.ps1` script found in the [exercises/scripts](exercises/scripts) folder. Be aware that the `Teardown.ps1` script will reset the entire instance.
