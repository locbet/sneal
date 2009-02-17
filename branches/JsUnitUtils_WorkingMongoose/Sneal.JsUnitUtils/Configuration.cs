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

using Sneal.JsUnitUtils.Browsers;
using Sneal.JsUnitUtils.TestFileReaders;

namespace Sneal.JsUnitUtils
{
    /// <summary>
    /// Encompasses the configuration to bootstrap the JsUnit runner.
    /// </summary>
    public class Configuration
    {
        public With Browser = With.InternetExplorer;
        public string WebRootDirectory;
        public string TestFixtureRunnerPath;
        public int FixtureTimeoutInSeconds;
        private readonly AssignedTestFileReader testFileReader = new AssignedTestFileReader();

        /// <summary>
        /// Queues a JsUnit local file path to for testing.
        /// </summary>
        /// <param name="localPathToJsUnitTest">The full path to the HTML test file.</param>
        public void AddTestFixtureFile(string localPathToJsUnitTest)
        {
            testFileReader.AddTestFixtureFile(localPathToJsUnitTest);
        }

        public ITestFileReader TestFileReader
        {
            get { return testFileReader; }
        }
    }
}
