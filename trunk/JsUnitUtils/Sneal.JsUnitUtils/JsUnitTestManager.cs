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
using Sneal.JsUnitUtils.TestFileReaders;

namespace Sneal.JsUnitUtils
{
    /// <summary>
    /// Factory for generating new JsUnitTestRunner instances.  This is the
    /// main entry point to this library.
    /// </summary>
    public class JsUnitTestManager
    {
        private readonly DefaultKernel kernel = new DefaultKernel();
        private readonly string jsUnitLibDirectory;

        public JsUnitTestManager(string jsUnitLibDirectory)
        {
            this.jsUnitLibDirectory = jsUnitLibDirectory;

            kernel.Register(
                Component.For<ITemplates>()
                    .ImplementedBy<Templates>(),
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
                Component.For<JsUnitWebServer>(),
                Component.For<JsUnitTestRunner>());
        }

        public JsUnitTestRunner CreateJsUnitRunner(string directory)
        {
            return CreateJsUnitRunner(
                directory,
                With.InternetExplorer);
        }

        public JsUnitTestRunner CreateJsUnitRunner(string directory, With browser)
        {
            return CreateJsUnitRunner(
                new TestFileReader(directory),
                browser);
        }

        public JsUnitTestRunner CreateJsUnitRunner(ITestFileReader testReader)
        {
            return CreateJsUnitRunner(
                testReader,
                With.InternetExplorer);
        }

        public JsUnitTestRunner CreateJsUnitRunner(ITestFileReader testReader, With browser)
        {
            IDictionary args = new Hashtable();
            args["testFileReader"] = testReader;
            args["jsUnitLibDirectory"] = jsUnitLibDirectory;
            args["testFileReader"] = testReader;
            args["webBrowser"] = kernel.Resolve<IWebBrowser>(browser.ToString());

            return kernel.Resolve<JsUnitTestRunner>(args);          
        }


    }
}