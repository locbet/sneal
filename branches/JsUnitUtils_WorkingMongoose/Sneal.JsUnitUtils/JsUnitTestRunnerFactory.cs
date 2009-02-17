#region license
// Copyright 2008 Shawn Neal (sneal@sneal.net)
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion

using System.Collections;
using Castle.MicroKernel;
using Castle.MicroKernel.Registration;
using Sneal.JsUnitUtils.Browsers;
using Sneal.JsUnitUtils.Servers;
using Sneal.JsUnitUtils.Utils;

namespace Sneal.JsUnitUtils
{
    /// <summary>
    /// Factory for generating new JsUnitTestRunner instances.  This is the
    /// main entry point to this library.
    /// </summary>
    /// <example>
    /// var runnerFactory = new JsUnitTestRunnerFactory();
    /// var runner = runnerFactory.CreateRunner(testReader, webRootDirectory, With.InternetExplorer);
    /// runner.RunAllTests();
    /// </example>
    public class JsUnitTestRunnerFactory
    {
        private DefaultKernel kernel = new DefaultKernel();
        private Hashtable settings = new Hashtable();

        private void InitContainerDefaults()
        {
            settings.Clear();
            kernel.Register(
                Component.For<IFreeTcpPortFinder>()
                    .ImplementedBy<FreeTcpPortFinder>(),
                Component.For<IDiskProvider>()
                    .ImplementedBy<DiskProviderImpl>(),
                Component.For<IWebServer>()
                    .ImplementedBy<BuiltinWebServer>(),
                Component.For<IResultParser>()
                    .ImplementedBy<JsUnitResultParser>(),
                Component.For<IResultListener>()
                    .ImplementedBy<WebServerPostListener>(),
                Component.For<FixtureRunner>(),
                Component.For<JsUnitTestRunner>());
        }

        /// <summary>
        /// Creates a runner that will run all the test files specified.
        /// All tests should be located beneath the webRootDirectory.
        /// </summary>
        public JsUnitTestRunner CreateRunner(Configuration configuration)
        {
            if (string.IsNullOrEmpty(configuration.WebRootDirectory))
            {
                throw new JsUnitConfigurationException("WebRootDirectory is required");
            }

            InitContainerDefaults();

            RegisterBrowser(configuration);
            RegisterFixtureFinder(configuration);
            RegisterTestFileReader(configuration);
            
            return CreateRunnerImpl(configuration);
        }

        private JsUnitTestRunner CreateRunnerImpl(Configuration configuration)
        {
            settings.Add("webRootDirectory", configuration.WebRootDirectory);
            JsUnitTestRunner runner = kernel.Resolve<JsUnitTestRunner>(settings);
            if (configuration.FixtureTimeoutInSeconds > 0)
            {
                runner.FixtureTimeoutInSeconds = configuration.FixtureTimeoutInSeconds;
            }
            return runner;
        }

        private void RegisterTestFileReader(Configuration configuration)
        {
            kernel.Register(
                Component.For<ITestFileReader>().Instance(configuration.TestFileReader));
        }

        private void RegisterFixtureFinder(Configuration configuration)
        {
            if (!string.IsNullOrEmpty(configuration.TestFixtureRunnerPath))
            {
                settings.Add("fullyQualifiedLocalPath", configuration.TestFixtureRunnerPath);
                kernel.Register(
                    Component.For<IFixtureFinder>().ImplementedBy<AssignedFixtureFinder>());
            }
            else
            {
                kernel.Register(
                    Component.For<IFixtureFinder>().ImplementedBy<FixtureFinder>());
            }
        }

        private void RegisterBrowser(Configuration configuration)
        {
            if (configuration.Browser != With.InternetExplorer)
            {
                kernel.Register(
                    Component.For<IWebBrowser>().ImplementedBy<FireFoxBrowser>());
            }
            else
            {
                kernel.Register(
                    Component.For<IWebBrowser>().ImplementedBy<InternetExplorerBrowser>());
            }
        }
    }
}