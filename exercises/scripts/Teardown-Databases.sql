USE [master]
GO

-- Exercise-01-before

IF EXISTS (SELECT name FROM master.dbo.sysdatabases WHERE name = N'Exercise-01-before-sales')
DROP DATABASE [Exercise-01-before-sales]
GO

IF EXISTS (SELECT name FROM master.dbo.sysdatabases WHERE name = N'Exercise-01-before-customers')
DROP DATABASE [Exercise-01-before-customers]
GO

IF EXISTS (SELECT name FROM master.dbo.sysdatabases WHERE name = N'Exercise-01-before-finance')
DROP DATABASE [Exercise-01-before-finance]
GO

-- Exercise-01-after

IF EXISTS (SELECT name FROM master.dbo.sysdatabases WHERE name = N'Exercise-01-after-sales')
DROP DATABASE [Exercise-01-after-sales]
GO

IF EXISTS (SELECT name FROM master.dbo.sysdatabases WHERE name = N'Exercise-01-after-customers')
DROP DATABASE [Exercise-01-after-customers]
GO

IF EXISTS (SELECT name FROM master.dbo.sysdatabases WHERE name = N'Exercise-01-after-finance')
DROP DATABASE [Exercise-01-after-finance]
GO

-- Exercise-02-before

IF EXISTS (SELECT name FROM master.dbo.sysdatabases WHERE name = N'Exercise-02-before-customers')
DROP DATABASE [Exercise-02-before-customers]
GO

IF EXISTS (SELECT name FROM master.dbo.sysdatabases WHERE name = N'Exercise-02-before-sales')
DROP DATABASE [Exercise-02-before-sales]
GO

IF EXISTS (SELECT name FROM master.dbo.sysdatabases WHERE name = N'Exercise-02-before-finance')
DROP DATABASE [Exercise-02-before-finance]
GO

IF EXISTS (SELECT name FROM master.dbo.sysdatabases WHERE name = N'Exercise-02-before-shipping')
DROP DATABASE [Exercise-02-before-shipping]
GO

-- Exercise-02-after

IF EXISTS (SELECT name FROM master.dbo.sysdatabases WHERE name = N'Exercise-02-after-customers')
DROP DATABASE [Exercise-02-after-customers]
GO

IF EXISTS (SELECT name FROM master.dbo.sysdatabases WHERE name = N'Exercise-02-after-sales')
DROP DATABASE [Exercise-02-after-sales]
GO

IF EXISTS (SELECT name FROM master.dbo.sysdatabases WHERE name = N'Exercise-02-after-finance')
DROP DATABASE [Exercise-02-after-finance]
GO

IF EXISTS (SELECT name FROM master.dbo.sysdatabases WHERE name = N'Exercise-02-after-shipping')
DROP DATABASE [Exercise-02-after-shipping]
GO

-- Exercise-03-before

IF EXISTS (SELECT name FROM master.dbo.sysdatabases WHERE name = N'Exercise-03-before-customers')
DROP DATABASE [Exercise-03-before-customers]
GO

IF EXISTS (SELECT name FROM master.dbo.sysdatabases WHERE name = N'Exercise-03-before-sales')
DROP DATABASE [Exercise-03-before-sales]
GO

IF EXISTS (SELECT name FROM master.dbo.sysdatabases WHERE name = N'Exercise-03-before-finance')
DROP DATABASE [Exercise-03-before-finance]
GO

IF EXISTS (SELECT name FROM master.dbo.sysdatabases WHERE name = N'Exercise-03-before-shipping')
DROP DATABASE [Exercise-03-before-shipping]
GO

-- Exercise-03-after

IF EXISTS (SELECT name FROM master.dbo.sysdatabases WHERE name = N'Exercise-03-after-customers')
DROP DATABASE [Exercise-03-after-customers]
GO

IF EXISTS (SELECT name FROM master.dbo.sysdatabases WHERE name = N'Exercise-03-after-sales')
DROP DATABASE [Exercise-03-after-sales]
GO

IF EXISTS (SELECT name FROM master.dbo.sysdatabases WHERE name = N'Exercise-03-after-finance')
DROP DATABASE [Exercise-03-after-finance]
GO

IF EXISTS (SELECT name FROM master.dbo.sysdatabases WHERE name = N'Exercise-03-after-shipping')
DROP DATABASE [Exercise-03-after-shipping]
GO

-- Exercise-04-before

IF EXISTS (SELECT name FROM master.dbo.sysdatabases WHERE name = N'Exercise-04-before-customers')
DROP DATABASE [Exercise-04-before-customers]
GO

IF EXISTS (SELECT name FROM master.dbo.sysdatabases WHERE name = N'Exercise-04-before-sales')
DROP DATABASE [Exercise-04-before-sales]
GO

IF EXISTS (SELECT name FROM master.dbo.sysdatabases WHERE name = N'Exercise-04-before-finance')
DROP DATABASE [Exercise-04-before-finance]
GO

IF EXISTS (SELECT name FROM master.dbo.sysdatabases WHERE name = N'Exercise-04-before-shipping')
DROP DATABASE [Exercise-04-before-shipping]
GO

-- Exercise-04-after

IF EXISTS (SELECT name FROM master.dbo.sysdatabases WHERE name = N'Exercise-04-after-customers')
DROP DATABASE [Exercise-04-after-customers]
GO

IF EXISTS (SELECT name FROM master.dbo.sysdatabases WHERE name = N'Exercise-04-after-sales')
DROP DATABASE [Exercise-04-after-sales]
GO

IF EXISTS (SELECT name FROM master.dbo.sysdatabases WHERE name = N'Exercise-04-after-finance')
DROP DATABASE [Exercise-04-after-finance]
GO

IF EXISTS (SELECT name FROM master.dbo.sysdatabases WHERE name = N'Exercise-04-after-shipping')
DROP DATABASE [Exercise-04-after-shipping]
GO
