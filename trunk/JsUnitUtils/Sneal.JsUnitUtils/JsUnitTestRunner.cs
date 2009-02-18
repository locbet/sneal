﻿#region license
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

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Sneal.JsUnitUtils.Servers;
using Sneal.Preconditions.Aop;

namespace Sneal.JsUnitUtils
{
    /// <summary>
    /// Bootstraps the JSUnit runner.  Searches for all tests in a given
    /// path and collects the JSUnit results.
    /// </summary>
    public class JsUnitTestRunner
    {
        public int FixtureTimeoutInSeconds = 60;
        public string TestRunnerHtmlFileName = "testRunner.html";

        private readonly ITestFileReader testFileReader;
        private readonly FixtureRunner fixtureRunner;
        private readonly IWebServer webServer;
        private readonly IFixtureFinder fixtureFinder;
        private readonly List<JsUnitErrorResult> results = new List<JsUnitErrorResult>();

        public JsUnitTestRunner(
            [NotNull] IWebServer webServer,
            [NotNull] FixtureRunner fixtureRunner,
            [NotNull] ITestFileReader testFileReader,
            [NotNull] IFixtureFinder fixtureFinder)
        {
            this.webServer = webServer;
            this.fixtureRunner = fixtureRunner;
            this.testFileReader = testFileReader;
            this.fixtureFinder = fixtureFinder;
        }

        public bool RunAllTests()
        {
            results.Clear();

            using (webServer.Start())
            {
                foreach (string testFile in testFileReader)
                {
                    string testFileUrl = webServer.MakeHttpUrl(testFile);

                    if (!fixtureRunner.RunFixture(GetFixtureUrl(testFileUrl), FixtureTimeoutInMilliseconds))
                    {
                        results.AddRange(fixtureRunner.Errors);
                    }
                }
            }

            return HasErrors;
        }

        private Uri GetFixtureUrl([NotNullOrEmpty] string testFileHttpUri)
        {
            return new Uri(string.Format(
                "{0}?testpage={1}&autoRun=true&submitResults={2}"
                , fixtureFinder.GetTestRunnerPath()
                , testFileHttpUri
                , webServer.WebRootHttpPath));
        }

        public IList<JsUnitErrorResult> Results
        {
            get { return new ReadOnlyCollection<JsUnitErrorResult>(results); }
        }

        public bool HasErrors
        {
            get { return null == results.Find(o => o.IsError); }
        }

        private int FixtureTimeoutInMilliseconds
        {
            get { return FixtureTimeoutInSeconds*1000; }
        }
    }

}
