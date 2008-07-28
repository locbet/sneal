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
using System.Linq;
using System.Text;
using WatiN.Core;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Runtime.InteropServices;
using System.Threading;

namespace Sneal.JsUnitUtils
{
    public class JsUnitTestRunner
    {
        // Fields
        private readonly string errorFileName;
        private string handlerFileName;
        private IE ie;
        private Process jsUnitServer;
        private int jsUnitServerPort;
        private readonly ITestFileReader reader;
        private readonly ITestFileReader sourceReader;
        private readonly string suiteFileName;
        private readonly ITemplates templates;
        private int testRunTimeoutInSeconds;

        // Methods
        public JsUnitTestRunner(ITestFileReader reader, ITestFileReader sourceReader, string jsUnitInstallDir)
            : this(new FileInfo("all_tests.html").FullName, reader, sourceReader, jsUnitInstallDir)
        {
        }

        public JsUnitTestRunner(string suiteFileName, ITestFileReader reader, ITestFileReader sourceReader, string jsUnitInstallDir)
        {
            this.errorFileName = "JSUnitResults.txt";
            this.jsUnitServerPort = 0x2f12;
            this.testRunTimeoutInSeconds = 120;
            this.handlerFileName = "JSUnitResultHandler.ashx";
            this.suiteFileName = suiteFileName;
            this.reader = reader;
            this.sourceReader = sourceReader;
            this.templates = new Templates(jsUnitInstallDir);
        }

        private void BuildSuite()
        {
            using (TextWriter writer = File.CreateText(this.suiteFileName))
            {
                new SuiteBuilder(this.templates, this.reader, this.sourceReader).Write(writer);
            }
        }

        private void CloseIE()
        {
            if (this.ie != null)
            {
                this.ie.Close();
            }
        }

        private void CreateWebDirectoryContent(string webDir)
        {
            string path = Path.Combine(webDir, "bin");
            Directory.CreateDirectory(path);
            string sourceFileName = Assembly.GetExecutingAssembly().GetName().Name + ".dll";
            File.Copy(sourceFileName, Path.Combine(path, sourceFileName), true);
            this.templates.CreateAshxHandler(this.handlerFileName);
        }

        private static string ParseJSUnitErrors(string results)
        {
            StringBuilder builder = new StringBuilder();
            string pattern = "^\\S+://\\S+:(?<functionname>\\w+)\\|(?<timing>[\\w|.]+)\\|[F|E]\\|(?<stack>[\\s\\w\\(\\):>\"']+)";
            RegexOptions options = RegexOptions.IgnorePatternWhitespace | RegexOptions.Multiline | RegexOptions.IgnoreCase;
            MatchCollection matchs = new Regex(pattern, options).Matches(results);
            foreach (Match match in matchs)
            {
                builder.AppendLine(match.Groups["functionname"].Value);
                builder.AppendLine(match.Groups["stack"].Value);
                builder.AppendLine("-----------------------------------------------");
                builder.AppendLine();
            }
            return builder.ToString();
        }

        private void RemovePreviousRunErrors()
        {
            if (File.Exists(this.errorFileName))
            {
                File.Delete(this.errorFileName);
            }
        }

        public bool Run()
        {
            try
            {
                this.RemovePreviousRunErrors();
                this.StartJSUnitServer();
                this.BuildSuite();
                this.RunJSUnitTests();
                this.ThrowJSUnitErrors();
            }
            finally
            {
                this.StopJSUnitServer();
                this.CloseIE();
            }
            return true;
        }

        private void RunJSUnitTests()
        {
            string url = this.templates.CreateJsUnitUri(this.suiteFileName) + string.Format("&submitResults=localhost:{0}/JSUnitResultHandler.ashx", this.jsUnitServerPort);
            this.ie = new IE(url);
        }

        private void StartJSUnitServer()
        {
            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            this.CreateWebDirectoryContent(baseDirectory);
            string fileName = Path.Combine(RuntimeEnvironment.GetRuntimeDirectory(), "WebDev.WebServer.exe");
            string arguments = string.Format("/port:{0} /path:{1}", this.jsUnitServerPort, baseDirectory);
            this.jsUnitServer = Process.Start(fileName, arguments);
        }

        private void StopJSUnitServer()
        {
            if (!((this.jsUnitServer == null) || this.jsUnitServer.HasExited))
            {
                this.jsUnitServer.Kill();
            }
        }

        private void ThrowJSUnitErrors()
        {
            for (int i = 0; !File.Exists(this.errorFileName) && (i < this.testRunTimeoutInSeconds); i++)
            {
                Thread.Sleep(0x3e8);
            }
            if (!File.Exists(this.errorFileName))
            {
                throw new JSUnitTestFailureException(string.Format("Missing {0} intermediate results text file.  The JSUnit tests have probably timed out.", this.errorFileName));
            }
            using (StreamReader reader = File.OpenText(this.errorFileName))
            {
                string str2 = ParseJSUnitErrors(reader.ReadToEnd());
                if (!string.IsNullOrEmpty(str2))
                {
                    throw new JSUnitTestFailureException(str2);
                }
            }
        }
    }

 

}
