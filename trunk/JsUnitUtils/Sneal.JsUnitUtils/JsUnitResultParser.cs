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
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace Sneal.JsUnitUtils
{
    public class JsUnitResultParser : IResultParser
    {
        public IList<JsUnitErrorResult> ParseJsUnitErrors(string results)
        {
            var parsedResults = new List<JsUnitErrorResult>();

            if (results == null)
            {
                parsedResults.Add(new JsUnitErrorResult
                {
                    Message = "The test timed out while waiting for the results.",
                });
                return parsedResults;
            }

            const RegexOptions options = RegexOptions.IgnorePatternWhitespace | RegexOptions.Multiline | RegexOptions.IgnoreCase;
            const string pattern = 
                "^(?<testpage>\\S+://\\S+):(?<functionname>\\w+)\\|" + 
                "(?<timing>[\\w|.]+)\\|(?<result>[F|E|S])\\|" +
                "\\|?(?<message>[\\s\\w\\(\\):>\"']*)";

            // TODO: This is fragile and could produce unexpected results
            string[] resultParts = results.Split(new[] {","}, StringSplitOptions.RemoveEmptyEntries);
            foreach (var testResult in resultParts)
            {
                MatchCollection matches = new Regex(pattern, options).Matches(testResult);

                foreach (Match match in matches)
                {
                    parsedResults.Add(new JsUnitErrorResult
                    {
                        TestPage = match.Groups["testpage"].Value,
                        FunctionName = match.Groups["functionname"].Value,
                        Message = match.Groups["message"].Value,
                        Timing = match.Groups["timing"].Value,
                        TestResult = CharToTestResult(match.Groups["result"].Value)
                    });
                }                
            }

            return parsedResults;
        }

        private static TestResult CharToTestResult(string result)
        {
            switch (result)
            {
                case "E":
                    return TestResult.Error;
                case "F":
                    return TestResult.Failure;
                case "S":
                    return TestResult.Success;
            }

            return TestResult.None;
        }
    }
}
