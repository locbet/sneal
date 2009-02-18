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
        private string testRunnerHtmlPath;
        private bool disableAuth;
        private static readonly IFormatProvider resultFormatProvider = new JsUnitErrorFormatProvider();

        public override bool Execute()
        {
            string webConfigPath = Path.Combine(webRootDirectory, "Web.config");
            var authTaskHelper = new AuthTaskHelper(this);

            if (disableAuth)
            {
                authTaskHelper.DisableWebConfigAuthorization(webConfigPath);
            }

            Configuration configuration = new Configuration
            {
                Browser = browserType,
                TestFixtureRunnerPath = testRunnerHtmlPath,
                WebRootDirectory = webRootDirectory,
                FixtureTimeoutInSeconds = timeout
            };

            foreach (var testFile in testFiles)
            {
                configuration.AddTestFixtureFile(testFile.ItemSpec);
            }

            bool result;
            try
            {
                runner = new JsUnitTestRunnerFactory().CreateRunner(configuration);

                result = runner.RunAllTests();
                foreach (JsUnitErrorResult testResult in runner.Results)
                {
                    if (testResult.IsError)
                        Log.LogError(testResult.ToString(null, resultFormatProvider));
                    else
                        Log.LogMessage(testResult.ToString(null, resultFormatProvider));
                }
            }
            finally
            {
                if (disableAuth)
                {
                    authTaskHelper.RestoreWebConfig(webConfigPath);
                }
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
                var memStream = new MemoryStream();
                var writer = new StreamWriter(memStream);
                
                var results = new List<JsUnitErrorResult>(runner.Results);
                var ser = new XmlSerializer(results.GetType());
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
                browserType = string.Compare(value, "firefox", StringComparison.OrdinalIgnoreCase) == 0
                    ? With.FireFox
                    : With.InternetExplorer;
            }
        }

        /// <summary>
        /// Disables authorization if set to <c>true</c>.
        /// </summary>
        public bool DisableAuth
        {
            get { return disableAuth; }
            set { disableAuth = value; }
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
        /// The fully qualified (local) path to the jsunit testRunner.html file.
        /// </summary>
        public string TestRunnerHtmlPath
        {
            get { return testRunnerHtmlPath; }
            set { testRunnerHtmlPath = value; }
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
