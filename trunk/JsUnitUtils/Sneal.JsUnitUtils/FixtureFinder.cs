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

using Sneal.JsUnitUtils.Utils;
using Sneal.Preconditions;

namespace Sneal.JsUnitUtils
{
    /// <summary>
    /// Locates the JsUnit test runner html file within the specified
    /// webserver path.
    /// </summary>
    public class FixtureFinder
    {
        public string TestRunnerHtmlFileName = "testRunner.html";
        private readonly IWebServer webServer;
        private readonly IDiskProvider diskProvider;
        private string testRunnerPath;

        /// <summary>
        /// Creates a new JS Unit fixture finder instance.
        /// </summary>
        /// <param name="webServer">The web server instance to search under.</param>
        /// <param name="diskProvider"></param>
        public FixtureFinder(IWebServer webServer, IDiskProvider diskProvider)
        {
            Throw.If(webServer).IsNull();
            Throw.If(diskProvider).IsNull();
            this.webServer = webServer;
            this.diskProvider = diskProvider;
        }

        /// <summary>
        /// Locates the testRunner.html file underneath the given webserver and
        /// returns the HTTP path to the file.
        /// </summary>
        /// <remarks>
        /// If found, the path is cached within this instance.
        /// </remarks>
        /// <returns>The full HTTP path to the fixture runner html file.</returns>
        public virtual string GetTestRunnerPath()
        {
            if (testRunnerPath == null)
            {
                FindTestRunnerHttpPath();
            }

            return testRunnerPath;
        }

        protected virtual void FindTestRunnerHttpPath()
        {
            string jsUnitRunnerPath = diskProvider.FindFile(webServer.WebRootDirectory, TestRunnerHtmlFileName);
            if (jsUnitRunnerPath == null)
            {
                return;
            }

            // convert local path into http path
            // local path: c:\source\myproj\jsunit\testRunner.html
            // http path: http://localhost:6303/jsunit/testRunner.html

            string relativePath = diskProvider.MakePathRelative(webServer.WebRootDirectory, jsUnitRunnerPath); //jsUnitRunnerPath.Substring(webServer.WebRootDirectory.Length);
            testRunnerPath = diskProvider.Combine(webServer.WebRootHttpPath, relativePath);
        }
    }
}
