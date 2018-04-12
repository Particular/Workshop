USE [master]
GO

-- Exercise-01-before

:setvar DatabaseName "Exercise-01-before-customers"
IF  NOT EXISTS (SELECT name FROM master.dbo.sysdatabases WHERE name = N'$(DatabaseName)')
CREATE DATABASE [$(DatabaseName)] ON ( NAME = N'$(DatabaseName)', FILENAME = N'$(UserPath)\$(DatabaseName).mdf' )
GO

:setvar DatabaseName "Exercise-01-before-sales"
IF  NOT EXISTS (SELECT name FROM master.dbo.sysdatabases WHERE name = N'$(DatabaseName)')
CREATE DATABASE [$(DatabaseName)] ON ( NAME = N'$(DatabaseName)', FILENAME = N'$(UserPath)\$(DatabaseName).mdf' )
GO

:setvar DatabaseName "Exercise-01-before-finance"
IF  NOT EXISTS (SELECT name FROM master.dbo.sysdatabases WHERE name = N'$(DatabaseName)')
CREATE DATABASE [$(DatabaseName)] ON ( NAME = N'$(DatabaseName)', FILENAME = N'$(UserPath)\$(DatabaseName).mdf' )
GO

-- Exercise-01-after

:setvar DatabaseName "Exercise-01-after-customers"
IF  NOT EXISTS (SELECT name FROM master.dbo.sysdatabases WHERE name = N'$(DatabaseName)')
CREATE DATABASE [$(DatabaseName)] ON ( NAME = N'$(DatabaseName)', FILENAME = N'$(UserPath)\$(DatabaseName).mdf' )
GO

:setvar DatabaseName "Exercise-01-after-sales"
IF  NOT EXISTS (SELECT name FROM master.dbo.sysdatabases WHERE name = N'$(DatabaseName)')
CREATE DATABASE [$(DatabaseName)] ON ( NAME = N'$(DatabaseName)', FILENAME = N'$(UserPath)\$(DatabaseName).mdf' )
GO

:setvar DatabaseName "Exercise-01-after-finance"
IF  NOT EXISTS (SELECT name FROM master.dbo.sysdatabases WHERE name = N'$(DatabaseName)')
CREATE DATABASE [$(DatabaseName)] ON ( NAME = N'$(DatabaseName)', FILENAME = N'$(UserPath)\$(DatabaseName).mdf' )
GO

-- Exercise-02-before

:setvar DatabaseName "Exercise-02-before-customers"
IF  NOT EXISTS (SELECT name FROM master.dbo.sysdatabases WHERE name = N'$(DatabaseName)')
CREATE DATABASE [$(DatabaseName)] ON ( NAME = N'$(DatabaseName)', FILENAME = N'$(UserPath)\$(DatabaseName).mdf' )
GO

:setvar DatabaseName "Exercise-02-before-sales"
IF  NOT EXISTS (SELECT name FROM master.dbo.sysdatabases WHERE name = N'$(DatabaseName)')
CREATE DATABASE [$(DatabaseName)] ON ( NAME = N'$(DatabaseName)', FILENAME = N'$(UserPath)\$(DatabaseName).mdf' )
GO

:setvar DatabaseName "Exercise-02-before-finance"
IF  NOT EXISTS (SELECT name FROM master.dbo.sysdatabases WHERE name = N'$(DatabaseName)')
CREATE DATABASE [$(DatabaseName)] ON ( NAME = N'$(DatabaseName)', FILENAME = N'$(UserPath)\$(DatabaseName).mdf' )
GO

:setvar DatabaseName "Exercise-02-before-shipping"
IF  NOT EXISTS (SELECT name FROM master.dbo.sysdatabases WHERE name = N'$(DatabaseName)')
CREATE DATABASE [$(DatabaseName)] ON ( NAME = N'$(DatabaseName)', FILENAME = N'$(UserPath)\$(DatabaseName).mdf' )
GO

-- Exercise-02-after

:setvar DatabaseName "Exercise-02-after-customers"
IF  NOT EXISTS (SELECT name FROM master.dbo.sysdatabases WHERE name = N'$(DatabaseName)')
CREATE DATABASE [$(DatabaseName)] ON ( NAME = N'$(DatabaseName)', FILENAME = N'$(UserPath)\$(DatabaseName).mdf' )
GO

:setvar DatabaseName "Exercise-02-after-sales"
IF  NOT EXISTS (SELECT name FROM master.dbo.sysdatabases WHERE name = N'$(DatabaseName)')
CREATE DATABASE [$(DatabaseName)] ON ( NAME = N'$(DatabaseName)', FILENAME = N'$(UserPath)\$(DatabaseName).mdf' )
GO

:setvar DatabaseName "Exercise-02-after-finance"
IF  NOT EXISTS (SELECT name FROM master.dbo.sysdatabases WHERE name = N'$(DatabaseName)')
CREATE DATABASE [$(DatabaseName)] ON ( NAME = N'$(DatabaseName)', FILENAME = N'$(UserPath)\$(DatabaseName).mdf' )
GO

:setvar DatabaseName "Exercise-02-after-shipping"
IF  NOT EXISTS (SELECT name FROM master.dbo.sysdatabases WHERE name = N'$(DatabaseName)')
CREATE DATABASE [$(DatabaseName)] ON ( NAME = N'$(DatabaseName)', FILENAME = N'$(UserPath)\$(DatabaseName).mdf' )
GO

-- Exercise-03-before

:setvar DatabaseName "Exercise-03-before-customers"
IF  NOT EXISTS (SELECT name FROM master.dbo.sysdatabases WHERE name = N'$(DatabaseName)')
CREATE DATABASE [$(DatabaseName)] ON ( NAME = N'$(DatabaseName)', FILENAME = N'$(UserPath)\$(DatabaseName).mdf' )
GO

:setvar DatabaseName "Exercise-03-before-sales"
IF  NOT EXISTS (SELECT name FROM master.dbo.sysdatabases WHERE name = N'$(DatabaseName)')
CREATE DATABASE [$(DatabaseName)] ON ( NAME = N'$(DatabaseName)', FILENAME = N'$(UserPath)\$(DatabaseName).mdf' )
GO

:setvar DatabaseName "Exercise-03-before-finance"
IF  NOT EXISTS (SELECT name FROM master.dbo.sysdatabases WHERE name = N'$(DatabaseName)')
CREATE DATABASE [$(DatabaseName)] ON ( NAME = N'$(DatabaseName)', FILENAME = N'$(UserPath)\$(DatabaseName).mdf' )
GO

:setvar DatabaseName "Exercise-03-before-shipping"
IF  NOT EXISTS (SELECT name FROM master.dbo.sysdatabases WHERE name = N'$(DatabaseName)')
CREATE DATABASE [$(DatabaseName)] ON ( NAME = N'$(DatabaseName)', FILENAME = N'$(UserPath)\$(DatabaseName).mdf' )
GO

-- Exercise-03-after

:setvar DatabaseName "Exercise-03-after-customers"
IF  NOT EXISTS (SELECT name FROM master.dbo.sysdatabases WHERE name = N'$(DatabaseName)')
CREATE DATABASE [$(DatabaseName)] ON ( NAME = N'$(DatabaseName)', FILENAME = N'$(UserPath)\$(DatabaseName).mdf' )
GO

:setvar DatabaseName "Exercise-03-after-sales"
IF  NOT EXISTS (SELECT name FROM master.dbo.sysdatabases WHERE name = N'$(DatabaseName)')
CREATE DATABASE [$(DatabaseName)] ON ( NAME = N'$(DatabaseName)', FILENAME = N'$(UserPath)\$(DatabaseName).mdf' )
GO

:setvar DatabaseName "Exercise-03-after-finance"
IF  NOT EXISTS (SELECT name FROM master.dbo.sysdatabases WHERE name = N'$(DatabaseName)')
CREATE DATABASE [$(DatabaseName)] ON ( NAME = N'$(DatabaseName)', FILENAME = N'$(UserPath)\$(DatabaseName).mdf' )
GO

:setvar DatabaseName "Exercise-03-after-shipping"
IF  NOT EXISTS (SELECT name FROM master.dbo.sysdatabases WHERE name = N'$(DatabaseName)')
CREATE DATABASE [$(DatabaseName)] ON ( NAME = N'$(DatabaseName)', FILENAME = N'$(UserPath)\$(DatabaseName).mdf' )
GO

-- Exercise-04-before

:setvar DatabaseName "Exercise-04-before-customers"
IF  NOT EXISTS (SELECT name FROM master.dbo.sysdatabases WHERE name = N'$(DatabaseName)')
CREATE DATABASE [$(DatabaseName)] ON ( NAME = N'$(DatabaseName)', FILENAME = N'$(UserPath)\$(DatabaseName).mdf' )
GO

:setvar DatabaseName "Exercise-04-before-sales"
IF  NOT EXISTS (SELECT name FROM master.dbo.sysdatabases WHERE name = N'$(DatabaseName)')
CREATE DATABASE [$(DatabaseName)] ON ( NAME = N'$(DatabaseName)', FILENAME = N'$(UserPath)\$(DatabaseName).mdf' )
GO

:setvar DatabaseName "Exercise-04-before-finance"
IF  NOT EXISTS (SELECT name FROM master.dbo.sysdatabases WHERE name = N'$(DatabaseName)')
CREATE DATABASE [$(DatabaseName)] ON ( NAME = N'$(DatabaseName)', FILENAME = N'$(UserPath)\$(DatabaseName).mdf' )
GO

:setvar DatabaseName "Exercise-04-before-shipping"
IF  NOT EXISTS (SELECT name FROM master.dbo.sysdatabases WHERE name = N'$(DatabaseName)')
CREATE DATABASE [$(DatabaseName)] ON ( NAME = N'$(DatabaseName)', FILENAME = N'$(UserPath)\$(DatabaseName).mdf' )
GO

:setvar DatabaseName "Exercise-04-before-itops"
IF  NOT EXISTS (SELECT name FROM master.dbo.sysdatabases WHERE name = N'$(DatabaseName)')
CREATE DATABASE [$(DatabaseName)] ON ( NAME = N'$(DatabaseName)', FILENAME = N'$(UserPath)\$(DatabaseName).mdf' )
GO

-- Exercise-04-after

:setvar DatabaseName "Exercise-04-after-customers"
IF  NOT EXISTS (SELECT name FROM master.dbo.sysdatabases WHERE name = N'$(DatabaseName)')
CREATE DATABASE [$(DatabaseName)] ON ( NAME = N'$(DatabaseName)', FILENAME = N'$(UserPath)\$(DatabaseName).mdf' )
GO

:setvar DatabaseName "Exercise-04-after-sales"
IF  NOT EXISTS (SELECT name FROM master.dbo.sysdatabases WHERE name = N'$(DatabaseName)')
CREATE DATABASE [$(DatabaseName)] ON ( NAME = N'$(DatabaseName)', FILENAME = N'$(UserPath)\$(DatabaseName).mdf' )
GO

:setvar DatabaseName "Exercise-04-after-finance"
IF  NOT EXISTS (SELECT name FROM master.dbo.sysdatabases WHERE name = N'$(DatabaseName)')
CREATE DATABASE [$(DatabaseName)] ON ( NAME = N'$(DatabaseName)', FILENAME = N'$(UserPath)\$(DatabaseName).mdf' )
GO

:setvar DatabaseName "Exercise-04-after-shipping"
IF  NOT EXISTS (SELECT name FROM master.dbo.sysdatabases WHERE name = N'$(DatabaseName)')
CREATE DATABASE [$(DatabaseName)] ON ( NAME = N'$(DatabaseName)', FILENAME = N'$(UserPath)\$(DatabaseName).mdf' )
GO

:setvar DatabaseName "Exercise-04-after-itops"
IF  NOT EXISTS (SELECT name FROM master.dbo.sysdatabases WHERE name = N'$(DatabaseName)')
CREATE DATABASE [$(DatabaseName)] ON ( NAME = N'$(DatabaseName)', FILENAME = N'$(UserPath)\$(DatabaseName).mdf' )
GO
