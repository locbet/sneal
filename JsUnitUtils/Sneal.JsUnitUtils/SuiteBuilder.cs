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
using System.IO;
using System.Text.RegularExpressions;

namespace Sneal.JsUnitUtils
{
    public class SuiteBuilder
    {
        // Fields
        private int functionEntryIndex;
        private readonly ITestFileReader reader;
        private readonly ITestFileReader sourceReader;
        private readonly ITemplates templates;

        // Methods
        public SuiteBuilder(ITemplates templates, ITestFileReader reader, ITestFileReader sourceReader)
        {
            this.templates = templates;
            this.reader = reader;
            this.sourceReader = sourceReader;
        }

        private string GetExposedFunctions(string file)
        {
            StringBuilder builder = new StringBuilder();
            using (TextReader reader = File.OpenText(file))
            {
                for (string str = reader.ReadLine(); str != null; str = reader.ReadLine())
                {
                    string pattern = @"^\s*function\s+(?<functionname>test\w+)";
                    RegexOptions options = RegexOptions.IgnorePatternWhitespace | RegexOptions.Multiline | RegexOptions.IgnoreCase;
                    Match match = new Regex(pattern, options).Match(str);
                    if (match.Success)
                    {
                        builder.AppendFormat(this.templates.CreateExposedTestFunctionEntry(this.functionEntryIndex, match.Groups["functionname"].Value), new object[0]);
                        this.functionEntryIndex++;
                    }
                }
            }
            return builder.ToString();
        }

        public void Write(TextWriter writer)
        {
            string str;
            StringBuilder builder = new StringBuilder();
            StringBuilder builder2 = new StringBuilder();
            this.functionEntryIndex = 0;
            while ((str = this.reader.GetNextTestFile()) != null)
            {
                builder.AppendFormat(this.templates.CreateJavaScriptInclude(str), new object[0]).AppendLine();
                builder2.AppendFormat(this.GetExposedFunctions(str), new object[0]).AppendLine();
            }
            while ((str = this.sourceReader.GetNextTestFile()) != null)
            {
                builder.AppendFormat(this.templates.CreateJavaScriptInclude(str), new object[0]).AppendLine();
            }
            string exposeTestFunctionNames = this.templates.CreateExposedTestFunctionBlock(builder2.ToString());
            writer.Write(this.templates.CreateSuite(builder.ToString(), exposeTestFunctionNames));
        }
    }

 

}
