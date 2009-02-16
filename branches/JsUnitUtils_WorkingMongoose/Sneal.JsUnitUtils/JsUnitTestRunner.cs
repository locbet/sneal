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
        private readonly JsUnitResultServer resultWebServer;
        private readonly IWebServer testWebServer;
        private readonly IFixtureFinder fixtureFinder;
        private readonly List<JsUnitErrorResult> results = new List<JsUnitErrorResult>();

        public JsUnitTestRunner(
            [NotNull] JsUnitResultServer resultWebServer,
            [NotNull] IWebServer testWebServer,
            [NotNull] FixtureRunner fixtureRunner,
            [NotNull] ITestFileReader testFileReader,
            [NotNull] IFixtureFinder fixtureFinder)
        {
            this.resultWebServer = resultWebServer;
            this.testWebServer = testWebServer;
            this.fixtureRunner = fixtureRunner;
            this.testFileReader = testFileReader;
            this.fixtureFinder = fixtureFinder;
        }

        public bool RunAllTests()
        {
            results.Clear();

            using (resultWebServer.Start())
            {
                using (testWebServer.Start())
                {
                    foreach (string testFile in testFileReader)
                    {
                        string testFileUrl = testWebServer.MakeHttpUrl(testFile);

                        if (!fixtureRunner.RunFixture(GetFixtureUrl(testFileUrl), FixtureTimeoutInSeconds*1000))
                        {
                            results.AddRange(fixtureRunner.Errors);
                        }
                    }
                }
            }

            return results.Count == 0;
        }

        private Uri GetFixtureUrl([NotNullOrEmpty] string testFileHttpUri)
        {
            return new Uri(string.Format(
                "{0}?testpage={1}&autoRun=true&submitResults={2}"
                , fixtureFinder.GetTestRunnerPath()
                , testFileHttpUri
                , resultWebServer.HandlerAddress));
        }

        public IList<JsUnitErrorResult> Errors
        {
            get { return new ReadOnlyCollection<JsUnitErrorResult>(results); }
        }
    }

}
