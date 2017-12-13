USE [master]
GO

-- Exercise-01-before

IF  NOT EXISTS (SELECT name FROM master.dbo.sysdatabases WHERE name = N'Exercise-01-before-sales')
CREATE DATABASE [Exercise-01-before-sales]
GO

-- Exercise-01-after

IF  NOT EXISTS (SELECT name FROM master.dbo.sysdatabases WHERE name = N'Exercise-01-after-sales')
CREATE DATABASE [Exercise-01-after-sales]
GO

IF  NOT EXISTS (SELECT name FROM master.dbo.sysdatabases WHERE name = N'Exercise-01-after-customers')
CREATE DATABASE [Exercise-01-after-customers]
GO

IF  NOT EXISTS (SELECT name FROM master.dbo.sysdatabases WHERE name = N'Exercise-01-after-finance')
CREATE DATABASE [Exercise-01-after-finance]
GO

-- Exercise-02-before

IF  NOT EXISTS (SELECT name FROM master.dbo.sysdatabases WHERE name = N'Exercise-02-before-customers')
CREATE DATABASE [Exercise-02-before-customers]
GO

IF  NOT EXISTS (SELECT name FROM master.dbo.sysdatabases WHERE name = N'Exercise-02-before-sales')
CREATE DATABASE [Exercise-02-before-sales]
GO

IF  NOT EXISTS (SELECT name FROM master.dbo.sysdatabases WHERE name = N'Exercise-02-before-finance')
CREATE DATABASE [Exercise-02-before-finance]
GO

IF  NOT EXISTS (SELECT name FROM master.dbo.sysdatabases WHERE name = N'Exercise-02-before-shipping')
CREATE DATABASE [Exercise-02-before-shipping]
GO

-- Exercise-02-after

IF  NOT EXISTS (SELECT name FROM master.dbo.sysdatabases WHERE name = N'Exercise-02-after-customers')
CREATE DATABASE [Exercise-02-after-customers]
GO

IF  NOT EXISTS (SELECT name FROM master.dbo.sysdatabases WHERE name = N'Exercise-02-after-sales')
CREATE DATABASE [Exercise-02-after-sales]
GO

IF  NOT EXISTS (SELECT name FROM master.dbo.sysdatabases WHERE name = N'Exercise-02-after-finance')
CREATE DATABASE [Exercise-02-after-finance]
GO

IF  NOT EXISTS (SELECT name FROM master.dbo.sysdatabases WHERE name = N'Exercise-02-after-shipping')
CREATE DATABASE [Exercise-02-after-shipping]
GO

-- Exercise-03-before

IF  NOT EXISTS (SELECT name FROM master.dbo.sysdatabases WHERE name = N'Exercise-03-before-customers')
CREATE DATABASE [Exercise-03-before-customers]
GO

IF  NOT EXISTS (SELECT name FROM master.dbo.sysdatabases WHERE name = N'Exercise-03-before-sales')
CREATE DATABASE [Exercise-03-before-sales]
GO

IF  NOT EXISTS (SELECT name FROM master.dbo.sysdatabases WHERE name = N'Exercise-03-before-finance')
CREATE DATABASE [Exercise-03-before-finance]
GO

IF  NOT EXISTS (SELECT name FROM master.dbo.sysdatabases WHERE name = N'Exercise-03-before-shipping')
CREATE DATABASE [Exercise-03-before-shipping]
GO

-- Exercise-03-after

IF  NOT EXISTS (SELECT name FROM master.dbo.sysdatabases WHERE name = N'Exercise-03-after-customers')
CREATE DATABASE [Exercise-03-after-customers]
GO

IF  NOT EXISTS (SELECT name FROM master.dbo.sysdatabases WHERE name = N'Exercise-03-after-sales')
CREATE DATABASE [Exercise-03-after-sales]
GO

IF  NOT EXISTS (SELECT name FROM master.dbo.sysdatabases WHERE name = N'Exercise-03-after-finance')
CREATE DATABASE [Exercise-03-after-finance]
GO

IF  NOT EXISTS (SELECT name FROM master.dbo.sysdatabases WHERE name = N'Exercise-03-after-shipping')
CREATE DATABASE [Exercise-03-after-shipping]
GO

-- Exercise-04-before

IF  NOT EXISTS (SELECT name FROM master.dbo.sysdatabases WHERE name = N'Exercise-04-before-customers')
CREATE DATABASE [Exercise-04-before-customers]
GO

IF  NOT EXISTS (SELECT name FROM master.dbo.sysdatabases WHERE name = N'Exercise-04-before-sales')
CREATE DATABASE [Exercise-04-before-sales]
GO

IF  NOT EXISTS (SELECT name FROM master.dbo.sysdatabases WHERE name = N'Exercise-04-before-finance')
CREATE DATABASE [Exercise-04-before-finance]
GO

IF  NOT EXISTS (SELECT name FROM master.dbo.sysdatabases WHERE name = N'Exercise-04-before-shipping')
CREATE DATABASE [Exercise-04-before-shipping]
GO

-- Exercise-04-after

IF  NOT EXISTS (SELECT name FROM master.dbo.sysdatabases WHERE name = N'Exercise-04-after-customers')
CREATE DATABASE [Exercise-04-after-customers]
GO

IF  NOT EXISTS (SELECT name FROM master.dbo.sysdatabases WHERE name = N'Exercise-04-after-sales')
CREATE DATABASE [Exercise-04-after-sales]
GO

IF  NOT EXISTS (SELECT name FROM master.dbo.sysdatabases WHERE name = N'Exercise-04-after-finance')
CREATE DATABASE [Exercise-04-after-finance]
GO

IF  NOT EXISTS (SELECT name FROM master.dbo.sysdatabases WHERE name = N'Exercise-04-after-shipping')
CREATE DATABASE [Exercise-04-after-shipping]
GO
