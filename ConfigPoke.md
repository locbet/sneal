# Introduction #

ConfigPoke is an inheritance based configuration file tool that takes a chain of inherited key/value pairs and applies them to any text file (XML, SQL, etc). This allows you to manage configuration settings that vary between environment, user, computer, or whatever else you can think of.


## ConfigPoke Visual Studio 2005/2008 and MSBuild Integration ##

This explains how you can tightly integrate ConfigPoke into your Visual Studio builds so that per developer configuration happens automatically and is always up to date with the input template and property files. There is also a command line runner that you can use from your deployment scripts or installer.

With Visual Studio integration, the output configuration files are conditionally built only if the input templates file or input property(s) files are changed, just like any regular source file in Visual Studio.  If a Rebuild action is initiated in Visual Studio, the output configuration files are always rebuilt, even if the input files have not changed.


## Visual Studio 2005/2008 Integration Outline ##

  1. Import Sneal.Build.ConfigPoke.targets file into your VS project file.
  1. Create a ConfigTemplateFiles ItemGroup.
  1. Create a ConfigPropertyFiles ItemGroup.


## Detailed Visual Studio 2005/2008 Integration Instructions ##

Include the Sneal.Build.ConfigPoke.targets file in your Visual Studio project file (csproj) using the following element:
```
<Import Project="Sneal.Build.ConfigPoke.targets" />
```
With the ConfigPoke MSBuild targets file included, the configuration building will automatically be hooked into the Visual Studio clean, build, rebuild process.

The Sneal.Build.ConfigPoke.targets file requires two MSBuild item groups as input: ConfigTemplateFiles, and ConfigPropertyFiles.  These must be specified in your
MSBuild (csproj) project file.  Each ConfigTemplateFiles item should be the destination filename + some arbitrary extension, I use '.template'.  You may also probably want to set the "InProject" item meta data to false so these templates or property files don't show up in VS solution explorer.

Here's an example ConfigTemplateFiles ItemGroup that you would put into your csproj file:
```
<ItemGroup>
  <ConfigTemplateFiles Include="$(MSBuildProjectDirectory)\Configs\web.config.template"/>
  <ConfigTemplateFiles Include="$(MSBuildProjectDirectory)\Configs\windsor.config.template" />
</ItemGroup>
```
To create user and machine specific overrides, you can include a base properties file and then optionally include a per user and per machine config, if they exist.  This would allow each developer to create their own override properties file which can then be optionally checked into source control.  To automatically set this up, you can take advantage of the built in MSBuild properties: USERNAME and COMPUTERNAME.

Here's an example ConfigPropertyFiles ItemGroup that you would put into your csproj file:
```
<ItemGroup>
  <ConfigPropertyFiles Include="$(MSBuildProjectDirectory)\Properties\App.Properties.base"/>
  <ConfigPropertyFiles Include="$(MSBuildProjectDirectory)\Properties\App.Properties.$(USERNAME)" Condition="Exists('$(MSBuildProjectDirectory)\Properties\App.Properties.$(USERNAME)')"/>
  <ConfigPropertyFiles Include="$(MSBuildProjectDirectory)\Properties\App.Properties.$(COMPUTERNAME)" Condition="Exists('$(MSBuildProjectDirectory)\Properties\App.Properties.$(COMPUTERNAME)')"/>  
</ItemGroup>
```
**Note: You cannot pass in non-existant property files to the ConfigPoke utility, hence the Condition check above.**

Optionally you can specify a ConfigPokeDirectory and a ConfigOutputDirectory property to override the default directories. The ConfigPokeDirectory property should point to the directory where Sneal.Build.ConfigPoke.MSBuild.dll is located on your machine,
if not specified this property will default to the current MSBuild project file directory.
The ConfigOutputDirectory property should point to the directory where the output configuration files are written too.  If not
specified this property will default to the current MSBuild project file directory.

For additional usage see the example in SVN, http://sneal.googlecode.com/svn/trunk/ConfigPoke/Sneal.Build.ConfigPoke.Example/Sneal.Build.ConfigPoke.Example.csproj