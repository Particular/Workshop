#Requires -RunAsAdministrator

$instanceName = "particular-workshop"

sqllocaldb stop $instanceName
sqllocaldb delete $instanceName

$databasesPath = "$ENV:UserProfile\particular-workshop-databases"
mkdir -Force $databasesPath
rm -Recurse $databasesPath