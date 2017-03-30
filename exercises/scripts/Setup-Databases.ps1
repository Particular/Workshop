#Requires -RunAsAdministrator

$instanceName = "microservices-workshop"

sqllocaldb create $instanceName
sqllocaldb share $instanceName $instanceName
sqllocaldb start $instanceName
sqllocaldb info $instanceName

$serverName = "(localdb)\" + $instanceName
sqlcmd -S $serverName -i ".\Setup-Databases.sql"