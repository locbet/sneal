#region license
// Copyright 2008 Shawn Neal (neal.shawn@gmail.com)
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

namespace Sneal.Build.ConfigPoke
{
    public class ConfigPropertiesCollection : Dictionary<string, string>
    {
        public ConfigPropertiesCollection AddPropertiesFromReader(IConfigPropertiesReader reader)
        {
            if (reader == null)
                throw new ArgumentNullException("reader", "The ConfigPropertiesReader cannot be null");

            ParsePropertyReader(reader);

            return this;
        }

        protected virtual void ParsePropertyReader(IConfigPropertiesReader reader)
        {
            char[] equals = {'='};
            string line = reader.ReadLine();
            int lineNumber = 0;

            while (line != null)
            {
                if (!IsCommentOrEmptyLine(line))
                {
                    string[] keyValuePair = line.Split(equals, 2);
                    AssertKeyValuePairsAreValid(keyValuePair, lineNumber);

                    string key = keyValuePair[0].Trim();
                    string val = keyValuePair[1].Trim();

                    if (ContainsKey(key))
                        Remove(key);

                    Add(key, val);
                }

                line = reader.ReadLine();
                lineNumber++;
            }
        }

        private static bool IsCommentOrEmptyLine(string line)
        {
            return (line.Trim().Length == 0 || line.Trim().StartsWith("#"));
        }

        protected virtual void AssertKeyValuePairsAreValid(string[] keyValuePair, int lineNumber)
        {
            if (keyValuePair.Length != 2)
            {
                throw new ConfigPokeException(
                    string.Format(
                        "Your properties file contains an invalid formatted line. Expected a key value pair separated by an equals sign (key=value) on line {0}",
                        lineNumber));
            }

            string key = keyValuePair[0].Trim();

            if (string.IsNullOrEmpty(key))
            {
                throw new ConfigPokeException(
                    string.Format("The property key on line {0} is empty, please correct this.", lineNumber));
            }
        }
    }
}