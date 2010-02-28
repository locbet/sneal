Stormwind - Agile/Lean tool for dev teams.

To build: go.bat

Getting started:
VS 2010 (beta2) and .NET 4.0 is required to build and run.
MySql 5.1 or SQL Server 2005/2008 is required for the website and integration tests.
ReSharper 5.0 beta is used for refactoring and enforcement of coding styles.

To build and run the website or integration tests:
1. Create a Stormwind database. Open the MySQL console and type: create database Stormwind;
2. Modify the db connection settings in the app.config(s) and web.config.
3. Build the solution via Visual Studio or from the command line via go.bat.
4. Run the website, currently we're using the VS built in web server.


Before starting some notes on MVC design guidelines.
http://weblogs.asp.net/rashid/archive/2009/04/01/asp-net-mvc-best-practices-part-1.aspx


Stormwind Projects
================================================
Stormwind - MVC web project, top level assembly. Contains: controllers, views, jscript, css.
Stormwind.Infrastructure - Contains infrastructure glue code that depends upon 3rd party libraries.
Stormwind.Resources - Contains compiled resource strings.
Stormwind.Core - Main project that contains business logic and models. Minimal external dependencies.
Stormwind.Bcl - Utilities, extension methods, and types that enhance the .NET BCL.  Very generic.

Stormwind.UnitTests - Unit tests for the web site project.
Stormwind.Core.UnitTests - Unit tests for the core business logic.
Stormwind.Infrastructure.UnitTests - Unit tests for the infrastructure code.
Stormwind.IntegrationTests - Slower tests, tests that likely hit the DB or filesystem.
Stormwind.AcceptenceTests - Tests the entire stack, usually through WatiN.


External libraries that make Stormwind possible
================================================
Autofac 1.4.4 for dependency injection and deterministic disposal.
AutofacContrib.Startable is used for starting our infrastructure services.
NHibernate 2.0.1 for DAL.
NHibernate.Linq for writing queries.
Castle.DynamicProxy2 - Used by NHibernate for lazy loaded entities.
FluentNHibernate 1.0 for mapping classes to DB tables.
Log4Net for any logging operations
NUnit is our testing framework.
Dynamic mocking is handled by RhinoMocks.
NUnitEx (FluentNUnit) extensions for readable assertions.
WatiN for in browser testing.
MySQL 5.1
Sneal.Core for .NET extension methods and gaurd assertions.
Sneal.TestUtils provides the wrapper around WebDev.WebServer.exe
Duck Typing Library by David Meyer
NGenerics library


NUnit and .NET 4
================================================
The NUnit config had to be modifed to get it to work with .NET 4.0.
The supportRuntime element is required for all of our .NET 4.0 DLLs to be
runnable under NUnit:

  <startup useLegacyV2RuntimeActivationPolicy="true">
    <supportedRuntime version="v4.0.21006"/>
  </startup>

The GUI and console runner checked into the repository under tools already
have these modifications.