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
using System.Text;
using System.Collections.Generic;
using System.Xml.Serialization;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using Sneal.JsUnitUtils.Browsers;

namespace Sneal.JsUnitUtils.MsBuild
{
    /// <summary>
    /// Msbuild task for running JsUnit tests with IE or Firefox.
    /// </summary>
    public class JsUnitTask : Task
    {
        private ITaskItem[] testFiles;
        private With browserType = With.InternetExplorer;
        private JsUnitTestRunner runner;
        private int timeout = 60;
        private string webRootDirectory;

        public override bool Execute()
        {
            var reader = new TaskItemTestReader(testFiles);
            runner = new JsUnitTestRunnerFactory().CreateRunner(reader, webRootDirectory, browserType);

            bool result = runner.RunAllTests();

            foreach (JsUnitErrorResult error in runner.Errors)
            {
                string msg = string.Format("JsUnit {0} failed with message {1} in {2} milliseconds",
                                           error.FunctionName, error.StackTrace, error.Timing);
                Log.LogError(msg);
            }

            return result;
        }

        /// <summary>
        /// Really only relevant if ContinueOnError=true
        /// </summary>
        [Output]
        public string TestResults
        {
            get
            {
                MemoryStream memStream = new MemoryStream();
                StreamWriter writer = new StreamWriter(memStream);
                
                List<JsUnitErrorResult> results = new List<JsUnitErrorResult>(runner.Errors);
                XmlSerializer ser = new XmlSerializer(results.GetType());
                ser.Serialize(writer, results);
                writer.Flush();

                return Encoding.UTF8.GetString(memStream.GetBuffer());
            }
        }

        /// <summary>
        /// Optional browser property, defaults to IE.  Set this to "firefox" to
        /// run the tests with firefox.
        /// </summary>
        public string RunWith
        {
            get { return browserType.ToString(); }
            set
            {
                if (string.Compare(value, "firefox", StringComparison.OrdinalIgnoreCase) == 0)
                {
                    browserType = With.FireFox;
                }
                else
                {
                    browserType = With.InternetExplorer;
                }
            }
        }

        /// <summary>
        /// JSUnit runner timeout in seconds, defaults to 60 seconds.
        /// </summary>
        public int Timeout
        {
            get { return timeout; }
            set { timeout = value; }
        }

        /// <summary>
        /// The root directory of the web server, make sure you take into
        /// account any relative paths in your JsUnit test files.
        /// </summary>
        [Required]
        public string WebRootDirectory
        {
            get { return webRootDirectory; }
            set { webRootDirectory = value; }
        }

        /// <summary>
        /// Test file items to pass of to JsUnit.
        /// </summary>
        [Required]
        public ITaskItem[] TestFiles
        {
            get { return testFiles; }
            set { testFiles = value; }
        }
    }
}
