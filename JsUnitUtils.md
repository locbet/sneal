# JsUnitUtils #

The goal of this project is to make it super easy to include JsUnit tests as part of an automated build.

There's no need to setup a web server, or configure your application's web.config or anything else infrastructure related.  This project only needs to know where the tests are and where JsUnit is.  That's it.

The assembly includes its own web server for serving up the HTML test fixtures and the JavaScript to be tested.  This is done so that there is zero external configuration required to get the tests running.  Depending on your application architecture, its very likely you would otherwise need a complete working site backed by a database.

Besides running as an automated build, you could include this as a target in a csproj file to get the tests to run on build from Visual Studio.  Currently we only support an MSBuild task, although a console app or NAnt task could be added in the future.

Currently only Firefox and Internet Explorer are supported by this library, but it would fairly easy to add additional browsers.  Some refactoring needs to be done in this area to make IWebBrowser implementations easier to add from an external assembly.

## Using JsUnitUtils from MSBuild ##

To use the JsUnit test task, copy the Sneal.JsUnitUtils.MsBuild.dll and the Sneal.JsUnitUtils.targets file to somewhere where you can easily reference them from your MSBuild script.

Using the test task is pretty self explanatory, so I'll just show a quick example:

```
<PropertyGroup>
  <!-- Directory where the targets and DLL are located -->
  <SnealJsUnitUtilsPath>$(MSBuildProjectDirectory)</SnealJsUnitUtilsPath>
</PropertyGroup>

<!-- Include the JsUnitTask targets file -->
<Import Project="$(MSBuildProjectDirectory)\Sneal.JsUnitUtils.targets"/>

<!-- List of all test fixtures to run -->
<ItemGroup>
  <TestFiles Include="$(MSBuildProjectDirectory)\JsUnitTests\*.htm"/>
</ItemGroup>

<Target Name="TestWithIE">
  <Message Text="Running JsUnitTask with IE..."/>
  <JsUnitTask
      RunWith="IE"
      WebRootDirectory="$(MSBuildProjectDirectory)"
      TestFiles="@(TestFiles)"
      TimeOut="60"
      TestResultsPath="$(MSBuildProjectDirectory)\IETestResults.xml">
  </JsUnitTask>
</Target>
```

You must first set a property named SnealJsUnitUtilsPath, which should point to the directory that contains the Sneal.JsUnitUtils.targets and Sneal.JsUnitUtils.MsBuild.dll. With that set you can now import the targets file, which will automatically make the JsUnitTask task available for use from your MSBuild script.

### JsUnitTask attributes ###

  1. RunWith - Can be **IE** or **Firefox**, the default is IE.
  1. WebRootDirectory - This is generally the same directory as where your web.config is found.  This is required.
  1. TestFiles - 1-N list of test fixture HTML files to run.  This is required.
  1. TimeOut - Timeout in seconds.  The default is 5 minutes.
  1. TestResultsPath - If set the task will write the results to an XML file.  This is useful for displaying results in a CI dashboard.
  1. ContinueOnError - Same as other MSBuild tasks, true or false.
  1. TestRunnerHtmlPath - The path to JsUnit's testRunner.html file. This is optional as the web root directory is searched recursively for the test runner.
  1. TestResults - This is an optional output property.  The output is in XML format.

The web root directory attribute is required, this is generally the same directory as where your web.config is found.  This is needed so that runner can resolve relative paths of scripts included in the test fixtures. The task also need a list (1-N) of test fixtures to run.  Test fixtures are the html files that contain the actual tests; generally an ItemGroup is appropriate here.

The path to the JsUnit test runner html file is optional, since by default the task will recursively search for it underneath the root web directory.  The browser type is also optional, and is only required for Firefox since the default is to use IE.  The timeout attribute is optional and defaults to 5 minutes.

_For a complete working example see the Sneal.JsUnitUtils.MsBuild.Test.proj in SVN._

## Permissions ##

The account that the automated build runs under (or from the command line) needs to have administrator rights on the local Windows box.  Unfortunately the web server requires this to be able to run.

## Internet Explorer Notes ##

Sometimes IE likes to ask "Would you like to make IE your default browser?"  These dialogs are bad and will cause the build process to timeout without user intervention.  Since our goal is to completely automate JsUnit tests, we need to make sure IE doesn't prompt.

To smooth these issues over, first of all we need to open IE under the user account the tests will run under. If its your current account you're logged in with, then just start IE and tell it not to prompt you about making it the default browser.

If the tests will run under a different account than the one you're signed in with, then you need to use the runas command from the command prompt and start IE. This will create a IE profile for the user.

```
runas /user:mydomain\myserviceaccount "C:\Program Files\Internet Explorer\iexplore.exe"
```

## Firefox Notes ##

The build

To get the runner to run cleanly without timing out there are some additional Firefox settings required.  Without these, the build will hang (without user intervention) because of dialog popups asking things like, "Would you like to make Firefox your default browser" or "Would you like to restore your previous session?".  Any dialog waiting for user intervention will break the runner.

To smooth these issues over, first of all we need to open Firefox under the user account the tests will run under. If its your current account you're logged in with, then just start Firefox and tell it not to prompt you about making it the default browser.

If the tests will run under a different account than the one you're signed in with, then you need to use the runas command from the command prompt and start Firefox. This will create a Firefox profile for the user.

```
runas /user:mydomain\myserviceaccount "C:\Program Files\Mozilla Firefox\firefox.exe"
```

Once Firefox starts there's an additional setting required to keep it from asking to restore your session. To do this, you need to open the Firefox config page. Go to your about:config file by typing about "about:config" (without the quotes) in the address bar and press enter. Right click any part of the page, select "New," than "Boolean". A popup should appear asking for the preference name. Enter "browser.sessionstore.resume\_from\_crash" (without the quotes) . Select "false" as the value. Close Firefox to save settings.