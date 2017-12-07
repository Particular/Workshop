#Requires -RunAsAdministrator

$instanceName = "microservices-workshop"

sqllocaldb create $instanceName
sqllocaldb share $instanceName $instanceName
sqllocaldb start $instanceName
sqllocaldb info $instanceName
