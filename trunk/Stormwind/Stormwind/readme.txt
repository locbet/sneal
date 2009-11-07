Stormwind - Agile/Lean tool for dev teams.

Getting started:
VS 2010 (beta2) and .NET 4.0 is required to build and run.
MySql 5.1 is required for the website and integration tests.
ReSharper 5.0 beta is used for refactoring and enforcement of coding styles.

To build and run the website:
1. Create a Stormwind database. Open the MySQL console and type: create database Stormwind;
2. Open the AppSettings.cs or web.config and change the connection string.
3. Build the solution via Visual Studio or from the command line, go.bat.
4. Run the website, currently we're using the VS built in web server.

To build and run the integration tests (see NUnit and .NET 4 notes below):
1. Create a StormwindIntegration database. Open the MySQL console and type: create database StormwindIntegration;
2. Open the AssemblySetup.cs and change the connection string.
3. Build the solution via Visual Studio or from the command line, go.bat.
4. Run the tests from NUnit.


Before starting some notes on MVC design guidelines.
http://weblogs.asp.net/rashid/archive/2009/04/01/asp-net-mvc-best-practices-part-1.aspx


External libraries that make Stormwind possible
================================================
.NET Common Service Locator, to avoid direct dependencies on a particular DI container.
For the acutal DI implmentation we use Autofac (currently).
NHibernate for DAL.
FluentNHibernate for mapping classes to DB tables.
FluentMigrator for upgrading and downgrading the DB between versions. (or perhaps Migrator.NET)
Log4Net for any logging operations
NUnit is our testing framework.
Dynamic mocking is handled by the Moq framework.
NUnitEx (FluentNUnit) extensions for readable assertions.
WatiN for functional testing.
MySQL 5.1


NUnit and .NET 4
================================================
The NUnit config had to be modifed to get it to work with .NET 4.0.
The supportRuntime element is required for all of our .NET 4.0 DLLs to be
runnable under NUnit:

  <startup useLegacyV2RuntimeActivationPolicy="true">
    <supportedRuntime version="v4.0.21006"/>
  </startup>

To get the ReSharper 5 unit test runner working see:
http://www.sneal.net/blog/2009/11/07/ReSharper5WontRunUnitTestsInVisualStudio2010.aspx