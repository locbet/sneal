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

using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace Sneal.JsUnitUtils
{
    public class JsUnitResultParser : IResultParser
    {
        public IList<JsUnitErrorResult> ParseJsUnitErrors(string results)
        {
            var errors = new List<JsUnitErrorResult>();

            if (results == null)
            {
                errors.Add(new JsUnitErrorResult
                {
                    FunctionName = "Unknown",
                    StackTrace = "The test(s) timed out while waiting for the results."
                });
                return errors;
            }

            const RegexOptions options = RegexOptions.IgnorePatternWhitespace | RegexOptions.Multiline | RegexOptions.IgnoreCase;
            const string pattern = 
                "^\\S+://\\S+:(?<functionname>\\w+)\\|" + 
                "(?<timing>[\\w|.]+)\\|[F|E]\\|" + 
                "(?<stack>[\\s\\w\\(\\):>\"']+)";
            
            MatchCollection matches = new Regex(pattern, options).Matches(results);
            
            foreach (Match match in matches)
            {
                errors.Add(new JsUnitErrorResult
                {
                    FunctionName = match.Groups["functionname"].Value,
                    StackTrace = match.Groups["stack"].Value,
                    Timing =  match.Groups["timing"].Value
                });
            }

            return errors;
        }
    }
}
