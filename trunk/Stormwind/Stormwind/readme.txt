Stormwind - Agile/Lean tool for dev teams.

Getting started:
VS 2010 (beta2) and .NET 4.0 is required to build and run.

Before starting some notes on MVC design guidelines.
http://weblogs.asp.net/rashid/archive/2009/04/01/asp-net-mvc-best-practices-part-1.aspx


External libraries that make Stormwind possible:

.NET Common Service Locator, to avoid direct dependencies on a particular DI container.
For the acutal DI implmentation we use Autofac (currently).
NHibernate for DAL.
FluentNHibernate for mapping classes to DB tables.
FluentMigrator for upgrading and downgrading the DB between versions.
Log4Net for any logging operations
NUnit is our testing framework.
Dynamic mocking is handled by the Moq framework.
NUnitEx (FluentNUnit) extensions for readable assertions.
WatiN for functional testing.
