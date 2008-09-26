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
    public class JsUnitTask : Task
    {
        private string jsUnitDirectory;
        private ITaskItem[] testFiles;
        private With browserType = With.InternetExplorer;
        private JsUnitTestRunner runner;
        private int timeout = 60;

        public override bool Execute()
        {
            var reader = new TaskItemTestReader(testFiles);
            var mgr = new JsUnitTestManager(jsUnitDirectory);
            runner = mgr.CreateJsUnitRunner(reader, browserType);

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

        [Required]
        public string JsUnitDirectory
        {
            get { return jsUnitDirectory; }
            set { jsUnitDirectory = value; }
        }

        [Required]
        public ITaskItem[] TestFiles
        {
            get { return testFiles; }
            set { testFiles = value; }
        }
    }
}
