#Requires -RunAsAdministrator

$instanceName = "particular-workshop"

sqllocaldb create $instanceName
sqllocaldb share $instanceName $instanceName
sqllocaldb start $instanceName
sqllocaldb info $instanceName

$serverName = "(localdb)\" + $instanceName
$databasesPath = "$ENV:UserProfile\particular-workshop-databases"
mkdir -Force $databasesPath
$pathParameter = '"{0}"' -f $databasesPath
sqlcmd -S $serverName -v UserPath=$pathParameter -i "$PSScriptRoot\Setup-Databases.sql" 