#Requires -RunAsAdministrator

$instanceName = "microservices-workshop"

sqllocaldb stop $instanceName
sqllocaldb delete $instanceName
