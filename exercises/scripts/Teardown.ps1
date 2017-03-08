#Requires -RunAsAdministrator

$instanceName = "microservices-workshop"

$serverName = "(localdb)\" + $instanceName
sqlcmd -S $serverName -i ".\Teardown.sql"

sqllocaldb stop $instanceName
sqllocaldb delete $instanceName