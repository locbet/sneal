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

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;

namespace Sneal.JsUnitUtils
{
    /// <summary>
    /// Bootstraps the JSUnit runner.  Searches for all tests in a given
    /// path and collects the JSUnit results.
    /// </summary>
    public class JsUnitTestRunner
    {
        public int FixtureTimeoutInSeconds = 60;
        public string TestRunnerHtmlFilePath = "testRunner.html";

        private readonly ITestFileReader testFileReader;
        private readonly FixtureRunner fixtureRunner;
        private readonly string jsUnitLibDirectory;
        private readonly JsUnitWebServer jsUnitTestsWebServer;
        private readonly List<JsUnitErrorResult> results = new List<JsUnitErrorResult>();

        public JsUnitTestRunner(
            string jsUnitLibDirectory,
            JsUnitWebServer jsUnitTestsWebServer,
            FixtureRunner fixtureRunner,
            ITestFileReader testFileReader)
        {
            this.jsUnitLibDirectory = jsUnitLibDirectory;
            this.jsUnitTestsWebServer = jsUnitTestsWebServer;
            this.fixtureRunner = fixtureRunner;
            this.testFileReader = testFileReader;
        }

        public bool RunAllTests()
        {
            results.Clear();

            using (jsUnitTestsWebServer.Start())
            {
                ShadowCopyJsUnitToWebDirectory();

                foreach (string testFile in testFileReader)
                {
                    PrepareTestFile(testFile);
                    string testFileUrl = GetTestFileHttpUrl(testFile);

                    if (!fixtureRunner.RunFixture(GetFixtureUrl(testFileUrl), FixtureTimeoutInSeconds * 1000))
                    {
                        results.AddRange(fixtureRunner.Errors);
                    }
                }
            }

            return results.Count == 0;
        }

        private string GetTestFileHttpUrl(string testFile)
        {
            string testFileName = System.IO.Path.GetFileName(testFile);
            return Path.Combine(jsUnitTestsWebServer.WebRootHttpPath, testFileName);
        }

        private void PrepareTestFile(string testFile)
        {
            string testFileName = System.IO.Path.GetFileName(testFile);

            // shadow copy the test file
            string destTestFile = Path.Combine(
                jsUnitTestsWebServer.WebRootDirectory,
                testFileName);

            File.Copy(testFile, destTestFile, true);

            AppendJsUnitCoreScriptBlock(destTestFile);
        }

        private static void AppendJsUnitCoreScriptBlock(string destTestFile)
        {
            StreamWriter testFileStream = File.AppendText(destTestFile);
            testFileStream.WriteLine("<script type='text/javascript' src='app/jsUnitCore.js'></script>");
            testFileStream.Close();
        }

        private Uri GetFixtureUrl(string testFile)
        {
            return new Uri(string.Format(
                "http://localhost:{0}/{1}?testpage={2}&autoRun=true&submitResults={3}"
                , jsUnitTestsWebServer.WebServerPort
                , TestRunnerHtmlFilePath
                , testFile
                , jsUnitTestsWebServer.HandlerAddress));
        }

        private void ShadowCopyJsUnitToWebDirectory()
        {
            string testRunnerPath = Path.Combine(jsUnitLibDirectory, TestRunnerHtmlFilePath);
            if (!File.Exists(testRunnerPath))
            {
                throw new FileNotFoundException(
                    string.Format("Could not find the JSUnit test runner at {0}", testRunnerPath),
                    TestRunnerHtmlFilePath);
            }

            Directory.Copy(jsUnitLibDirectory, jsUnitTestsWebServer.WebRootDirectory);
        }

        public IList<JsUnitErrorResult> Errors
        {
            get { return new ReadOnlyCollection<JsUnitErrorResult>(results); }
        }
    }

}
