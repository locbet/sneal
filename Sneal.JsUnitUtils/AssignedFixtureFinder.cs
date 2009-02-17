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

using Sneal.JsUnitUtils.Servers;
using Sneal.Preconditions.Aop;

namespace Sneal.JsUnitUtils
{
    /// <summary>
    /// Allows the testRunner.html location to be explicitly specified.
    /// </summary>
    public class AssignedFixtureFinder : IFixtureFinder
    {
        private readonly IWebServer webServer;
        private readonly string fullyQualifiedLocalPath;

        /// <summary>
        /// Creates a new JS Unit fixture finder instance.
        /// </summary>
        /// <param name="webServer">The web server instance to search under.</param>
        /// <param name="fullyQualifiedLocalPath">
        /// The fully qualified local path to the JsUnit testRunner.html file.
        /// </param>
        public AssignedFixtureFinder([NotNull]IWebServer webServer, [NotNullOrEmpty] string fullyQualifiedLocalPath)
        {
            this.webServer = webServer;
            this.fullyQualifiedLocalPath = fullyQualifiedLocalPath;
        }

        public string GetTestRunnerPath()
        {
            return webServer.MakeHttpUrl(fullyQualifiedLocalPath);
        }
    }
}
