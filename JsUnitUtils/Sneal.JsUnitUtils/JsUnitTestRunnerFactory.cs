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
using System.Collections.Generic;
using Castle.MicroKernel;
using Castle.MicroKernel.Registration;
using Sneal.JsUnitUtils.Browsers;
using Sneal.JsUnitUtils.TestFileReaders;
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
        private readonly DefaultKernel kernel = new DefaultKernel();

        public JsUnitTestRunnerFactory()
        {
            kernel.Register(
                Component.For<ITemplates>()
                    .ImplementedBy<Templates>(),
                Component.For<IDiskProvider>()
                    .ImplementedBy<DiskProviderImpl>(),
                Component.For<IFixtureFinder>()
                    .ImplementedBy<FixtureFinder>(),
                Component.For<IWebServer>()
                    .ImplementedBy<JsUnitWebServer>(),
                Component.For<IWebBrowser>()
                    .ImplementedBy<InternetExplorerBrowser>()
                    .Named(With.InternetExplorer.ToString()),
                Component.For<IWebBrowser>()
                    .ImplementedBy<FireFoxBrowser>()
                    .Named(With.FireFox.ToString()),
                Component.For<IResultParser>()
                    .ImplementedBy<JsUnitResultParser>(),
                Component.For<IResultListener>()
                    .ImplementedBy<NamedPipesResultListener>(),
                Component.For<FixtureRunner>(),
                Component.For<JsUnitTestRunner>());
        }

        /// <summary>
        /// Creates a runner that will run all the test files specified.  All
        /// tests should be located beneath the webRootDirectory.
        /// </summary>
        /// <param name="testFiles">Full paths to the test files to hand off to JsUnit</param>
        /// <param name="webRootDirectory">The full path to the base webroot directory.</param>
        /// <param name="browser">The web browser type used to run the tests.</param>
        /// <returns>A ready to go JsUnit test runner instance.</returns>
        public JsUnitTestRunner CreateRunner(IEnumerable<string> testFiles, string webRootDirectory, With browser)
        {
            return CreateRunner(
                new TestFileReader(new List<string>(testFiles).ToArray()),
                webRootDirectory,
                browser);
        }

        /// <summary>
        /// Creates a runner that will run all the test files specified using IE.
        /// All tests should be located beneath the webRootDirectory.
        /// </summary>
        /// <param name="testReader">A test reader instance that provides tests.</param>
        /// <param name="webRootDirectory">The full path to the base webroot directory.</param>
        /// <returns>A ready to go JsUnit test runner instance.</returns>
        public JsUnitTestRunner CreateRunner(ITestFileReader testReader, string webRootDirectory)
        {
            return CreateRunner(
                testReader,
                webRootDirectory,
                With.InternetExplorer);
        }

        /// <summary>
        /// Creates a runner that will run all the test files specified.
        /// All tests should be located beneath the webRootDirectory.
        /// </summary>
        /// <param name="testReader">A test reader instance that provides tests.</param>
        /// <param name="webRootDirectory">The full path to the base webroot directory.</param>
        /// <param name="browser">The browser type used to run the tests.</param>
        /// <returns>A ready to go JsUnit test runner instance.</returns>
        public JsUnitTestRunner CreateRunner(ITestFileReader testReader, string webRootDirectory, With browser)
        {
            return CreateRunner(testReader, webRootDirectory, browser, null);
        }

        /// <summary>
        /// Creates a runner that will run all the test files specified.
        /// All tests should be located beneath the webRootDirectory.
        /// </summary>
        /// <param name="testReader">A test reader instance that provides tests.</param>
        /// <param name="webRootDirectory">The full path to the base webroot directory.</param>
        /// <param name="browser">The browser type used to run the tests.</param>
        /// <param name="fixturePath">
        /// The fully qualified JsUnit testRunner.html path, if not specified
        /// the file will be searched for under the web root directory.
        /// </param>
        /// <returns>A ready to go JsUnit test runner instance.</returns>
        public JsUnitTestRunner CreateRunner(ITestFileReader testReader, string webRootDirectory, With browser, string fixturePath)
        {
            var deps = new Hashtable
            {
                {"webRootDirectory", webRootDirectory},
                {"testFileReader", testReader},
                {"webBrowser", kernel.Resolve<IWebBrowser>(browser.ToString())},
            };

            // If a fixturePath is provided then override the default FixtureFinder.
            if (!string.IsNullOrEmpty(fixturePath))
            {
                deps.Add("fixtureFinder", new AssignedFixtureFinder(kernel.Resolve<IWebServer>(deps), fixturePath));
            }

            return kernel.Resolve<JsUnitTestRunner>(deps);
        }

    }
}