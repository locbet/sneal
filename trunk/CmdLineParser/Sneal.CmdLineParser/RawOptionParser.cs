#region license
// Copyright 2009 Shawn Neal (neal.shawn@gmail.com)
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

namespace Sneal.CmdLineParser
{
    public class RawOptionParser
    {
        /// <summary>
        /// Splits the current raw argument into a key value pair, where the
        /// key is the flag and the value is the commane line value.
        /// </summary>
        /// <param name="rawOption">The raw argument as entered by the user.</param>
        public KeyValuePair<string, string> SplitOptionNameAndValue(string rawOption)
        {
            string strippedArg = StripSwitchChar(rawOption);
            MatchCollection matches = OptionSplitRegex().Matches(strippedArg);
            if (matches.Count == 0 || matches[0].Groups.Count == 0)
            {
                return new KeyValuePair<string, string>();
            }

            string key = matches[0].Groups[1].Value;
            string value = matches[0].Groups[2].Value == "" ? null : matches[0].Groups[2].Value;
            Debug.Assert(key != null);

            return new KeyValuePair<string, string>(key, value);
        }

        private static Regex OptionSplitRegex()
        {
            const string regex = "^(\\w+)\\s?[:|=]?\\s?(.*)";
            const RegexOptions options = ((RegexOptions.IgnorePatternWhitespace | RegexOptions.Multiline) | RegexOptions.IgnoreCase);
            return new Regex(regex, options);
        }

        /// <summary>
        /// Removes any of the switch prefix characters from the raw argument.
        /// </summary>
        /// <remarks>
        /// input: /server=localhost
        /// output: server=localhost
        /// </remarks>
        /// <param name="rawArg">The raw argument.</param>
        /// <returns>The argument without the prefixed switch character.</returns>
        public string StripSwitchChar(string rawArg)
        {
            if (string.IsNullOrEmpty(rawArg))
            {
                return "";
            }

            rawArg = rawArg.Trim();
            foreach (char switchChar in CommandLineParser.SwitchChars)
            {
                if (rawArg[0] == switchChar)
                {
                    return rawArg.Substring(1);
                }
            }
            return rawArg;
        }
    }
}
