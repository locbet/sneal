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
using System.IO;
using NUnit.Framework;
using Sneal.JsUnitUtils.Browsers;
using Sneal.JsUnitUtils.TestFileReaders;

namespace Sneal.JsUnitUtils.IntegrationTests
{
    [TestFixture]
    public class ClientRunner
    {
        private string testDirectory;
        private string webRootDirectory;
        
        [SetUp]
        public void SetUp()
        {
            testDirectory = AppDomain.CurrentDomain.BaseDirectory + @"\..\..\JsUnitTests";
            webRootDirectory = AppDomain.CurrentDomain.BaseDirectory + @"\..\..\";
            Assert.IsTrue(File.Exists(Path.Combine(testDirectory, "JsUnitTestFixture1.htm")), "Cannot find test fixtures");

            CleanWebBinDIrectory();
        }

        [Test]
        public void Can_run_JSUnit_with_IE()
        {
            var mgr = new JsUnitTestRunnerFactory();

            // only pass html files that are not in the svn directory to the runner
            var testReader = new ExcludeTestFileReader("svn",
                new SuffixTestFileReader(".htm",
                    new TestFileReader(testDirectory)));

            var runner = mgr.CreateRunner(testReader, webRootDirectory, With.InternetExplorer);

            if (runner.RunAllTests())
            {
                Assert.Fail("There were no failing tests, there should be one that failed in JsUnitTestFixture1.htm");
            }

            AssertResults(runner);
        }

        [Test]
        public void Can_run_JSUnit_with_FireFox()
        {
            string[] testFiles =
            {
                testDirectory + "\\JsUnitTestFixture1.htm",
                testDirectory + "\\JsUnitTestFixture2.htm"
            };

            var mgr = new JsUnitTestRunnerFactory();
            var runner = mgr.CreateRunner(testFiles, webRootDirectory, With.FireFox);

            if (runner.RunAllTests())
            {
                Assert.Fail("There were no failing tests, there should be one that failed in JsUnitTestFixture1.htm");
            }

            AssertResults(runner);
        }

        private static void AssertResults(JsUnitTestRunner runner)
        {
            foreach (var error in runner.Errors)
            {
                Console.WriteLine(error.FunctionName);
                Console.WriteLine(error.StackTrace);
                Console.WriteLine(error.Timing);
            }   
         
            Assert.AreEqual(1, runner.Errors.Count, "Expected one error result message");
            Assert.IsTrue(runner.Errors[0].StackTrace.Contains("This test should fail"));
        }

        /// <summary>
        /// If the web bin directory contains both Sneal.JsUnitUtils.dll and
        /// Sneal.JsUnitUtils.MsBuild.dll the test will fail from an ambiguous
        /// result handler type.
        /// </summary>
        private void CleanWebBinDIrectory()
        {
            string msbuildFile = webRootDirectory + @"bin\Sneal.JsUnitUtils.MsBuild.dll";
            if (File.Exists(msbuildFile))
            {
                File.Delete(msbuildFile);
            }
        }
    }
}
