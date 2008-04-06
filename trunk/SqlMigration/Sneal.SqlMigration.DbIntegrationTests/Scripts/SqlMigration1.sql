USE [master]
GO
/****** Object:  Database [SqlMigration1]    Script Date: 04/05/2008 19:24:38 ******/
CREATE DATABASE [SqlMigration1]
GO


USE [SqlMigration1]
GO
/****** Object:  Table [dbo].[SoftwareVersion]    Script Date: 04/05/2008 19:17:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[SoftwareVersion]') AND OBJECTPROPERTY(id, N'IsUserTable') = 1)
BEGIN
CREATE TABLE [dbo].[SoftwareVersion](
	[Major] [int] NOT NULL,
	[Minor] [int] NOT NULL,
	[Build] [int] NOT NULL,
	[Revision] [int] NOT NULL,
	[DateOfBuild] [smalldatetime] NOT NULL,
 CONSTRAINT [PK_SoftwareVersion] PRIMARY KEY CLUSTERED 
(
	[Major] ASC,
	[Minor] ASC,
	[Build] ASC,
	[Revision] ASC
) ON [PRIMARY]
) ON [PRIMARY]
END
GO
IF NOT EXISTS (SELECT * FROM dbo.sysindexes WHERE id = OBJECT_ID(N'[dbo].[SoftwareVersion]') AND name = N'IX_SoftwareVersion')
CREATE NONCLUSTERED INDEX [IX_SoftwareVersion] ON [dbo].[SoftwareVersion] 
(
	[DateOfBuild] ASC
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Customer]    Script Date: 04/05/2008 19:17:14 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[Customer]') AND OBJECTPROPERTY(id, N'IsUserTable') = 1)
BEGIN
CREATE TABLE [dbo].[Customer](
	[CustomerID] [int] IDENTITY(1,1) NOT NULL,
	[FirstName] [nvarchar](50) NOT NULL,
	[LastName] [nvarchar](50) NOT NULL,
	[DataOfBirth] [smalldatetime] NOT NULL,
	[EmailAddress] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_Customer] PRIMARY KEY CLUSTERED 
(
	[CustomerID] ASC
) ON [PRIMARY]
) ON [PRIMARY]
END
GO
IF NOT EXISTS (SELECT * FROM dbo.sysindexes WHERE id = OBJECT_ID(N'[dbo].[Customer]') AND name = N'IX_Customer_EmailAddress')
CREATE UNIQUE NONCLUSTERED INDEX [IX_Customer_EmailAddress] ON [dbo].[Customer] 
(
	[EmailAddress] ASC
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Country]    Script Date: 04/05/2008 19:17:12 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[Country]') AND OBJECTPROPERTY(id, N'IsUserTable') = 1)
BEGIN
CREATE TABLE [dbo].[Country](
	[CountryID] [int] IDENTITY(1,1) NOT NULL,
	[Code] [nvarchar](3) NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_Country] PRIMARY KEY CLUSTERED 
(
	[CountryID] ASC
) ON [PRIMARY]
) ON [PRIMARY]
END
GO
/****** Object:  Table [dbo].[Address]    Script Date: 04/05/2008 19:17:11 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[Address]') AND OBJECTPROPERTY(id, N'IsUserTable') = 1)
BEGIN
CREATE TABLE [dbo].[Address](
	[AddressID] [int] IDENTITY(1,1) NOT NULL,
	[Street1] [nvarchar](50) NOT NULL,
	[Street2] [nvarchar](50) NULL,
	[City] [nvarchar](50) NOT NULL,
	[State] [nvarchar](50) NULL,
	[CountryID] [int] NOT NULL,
	[CustomerID] [int] NOT NULL,
	[IsPrimary] [bit] NOT NULL CONSTRAINT [DF_Address_IsPrimary]  DEFAULT ((1)),
 CONSTRAINT [PK_Address] PRIMARY KEY CLUSTERED 
(
	[AddressID] ASC
) ON [PRIMARY]
) ON [PRIMARY]
END
GO
/****** Object:  ForeignKey [FK_Address_Country]    Script Date: 04/05/2008 19:17:11 ******/
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[FK_Address_Country]') AND type = 'F')
ALTER TABLE [dbo].[Address]  WITH CHECK ADD  CONSTRAINT [FK_Address_Country] FOREIGN KEY([CountryID])
REFERENCES [dbo].[Country] ([CountryID])
GO
ALTER TABLE [dbo].[Address] CHECK CONSTRAINT [FK_Address_Country]
GO
/****** Object:  ForeignKey [FK_Address_Customer]    Script Date: 04/05/2008 19:17:11 ******/
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[FK_Address_Customer]') AND type = 'F')
ALTER TABLE [dbo].[Address]  WITH CHECK ADD  CONSTRAINT [FK_Address_Customer] FOREIGN KEY([CustomerID])
REFERENCES [dbo].[Customer] ([CustomerID])
GO
ALTER TABLE [dbo].[Address] CHECK CONSTRAINT [FK_Address_Customer]
GO

/****** Country Data ******/
SET IDENTITY_INSERT dbo.Country ON
INSERT INTO dbo.Country ([CountryID], [Code], [Name]) VALUES (1, 'US', 'United States')
SET IDENTITY_INSERT dbo.Country OFF

/****** Customer Data ******/
SET IDENTITY_INSERT dbo.Customer ON
INSERT INTO dbo.Customer ([CustomerID], [FirstName], [LastName], [DataOfBirth], [EmailAddress]) VALUES (1, 'Bill', 'Smith', '5/3/1988 12:00:00 AM', 'bsmith@nowhere.com')
INSERT INTO dbo.Customer ([CustomerID], [FirstName], [LastName], [DataOfBirth], [EmailAddress]) VALUES (2, 'Joe', 'Danube', '3/23/1945 12:00:00 AM', 'joethehitman@nowhere.com')
INSERT INTO dbo.Customer ([CustomerID], [FirstName], [LastName], [DataOfBirth], [EmailAddress]) VALUES (3, 'Jamie', 'Goldmine', '12/12/1965 12:00:00 AM', 'goldy@nowhere.com')
INSERT INTO dbo.Customer ([CustomerID], [FirstName], [LastName], [DataOfBirth], [EmailAddress]) VALUES (4, 'Mike', 'Gulpe', '1/12/1955 12:00:00 AM', 'bigdaddy@nowhere.com')
INSERT INTO dbo.Customer ([CustomerID], [FirstName], [LastName], [DataOfBirth], [EmailAddress]) VALUES (5, 'Jim', 'Claim', '5/24/1977 12:00:00 AM', 'claimjim@nowhere.com')
INSERT INTO dbo.Customer ([CustomerID], [FirstName], [LastName], [DataOfBirth], [EmailAddress]) VALUES (6, 'Adrian', 'Ardenne', '10/31/1982 12:00:00 AM', 'adrian35@nowhere.com')
SET IDENTITY_INSERT dbo.Customer OFF

/****** Address Data ******/
SET IDENTITY_INSERT dbo.Address ON
INSERT INTO dbo.Address ([AddressID], [Street1], [Street2], [City], [State], [CountryID], [CustomerID], [IsPrimary]) VALUES (1, '500 Main St.', 'Suite 200', 'Seattle', 'WA', 1, 1, 1)
INSERT INTO dbo.Address ([AddressID], [Street1], [Street2], [City], [State], [CountryID], [CustomerID], [IsPrimary]) VALUES (2, '9239 1st Ave.', NULL, 'New York', 'NY', 1, 2, 1)
INSERT INTO dbo.Address ([AddressID], [Street1], [Street2], [City], [State], [CountryID], [CustomerID], [IsPrimary]) VALUES (3, '377 Lakeside Lane.', NULL, 'Los Angeles', 'CA', 1, 2, 0)
INSERT INTO dbo.Address ([AddressID], [Street1], [Street2], [City], [State], [CountryID], [CustomerID], [IsPrimary]) VALUES (4, '838 4th Ave.', NULL, 'Tacoma', 'WA', 1, 3, 1)
INSERT INTO dbo.Address ([AddressID], [Street1], [Street2], [City], [State], [CountryID], [CustomerID], [IsPrimary]) VALUES (5, '45 Burlington Rd.', NULL, 'Kirkland', 'WA', 1, 4, 1)
INSERT INTO dbo.Address ([AddressID], [Street1], [Street2], [City], [State], [CountryID], [CustomerID], [IsPrimary]) VALUES (6, '27892 Woodinville-Duvalle Rd.', NULL, 'Woodinville', 'WA', 1, 5, 1)
INSERT INTO dbo.Address ([AddressID], [Street1], [Street2], [City], [State], [CountryID], [CustomerID], [IsPrimary]) VALUES (7, '401 1st Ave.', 'Second Floor', 'Seattle', 'WA', 1, 6, 1)
SET IDENTITY_INSERT dbo.Address OFF

/****** SoftwareVersion Data ******/
INSERT INTO dbo.SoftwareVersion ([Major], [Minor], [Build], [Revision], [DateOfBuild]) VALUES (1, 0, 0, 1303, '4/5/1999 12:00:00 AM')
INSERT INTO dbo.SoftwareVersion ([Major], [Minor], [Build], [Revision], [DateOfBuild]) VALUES (1, 1, 0, 1932, '4/30/1999 12:00:00 AM')
INSERT INTO dbo.SoftwareVersion ([Major], [Minor], [Build], [Revision], [DateOfBuild]) VALUES (1, 1, 5, 2043, '8/9/1999 12:00:00 AM')
INSERT INTO dbo.SoftwareVersion ([Major], [Minor], [Build], [Revision], [DateOfBuild]) VALUES (2, 0, 0, 7734, '11/10/2001 12:00:00 AM')
INSERT INTO dbo.SoftwareVersion ([Major], [Minor], [Build], [Revision], [DateOfBuild]) VALUES (2, 5, 0, 8553, '5/21/2002 12:00:00 AM')