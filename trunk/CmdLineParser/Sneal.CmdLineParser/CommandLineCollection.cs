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

using System;
using System.Collections.Generic;

namespace Sneal.CmdLineParser
{
    public class CommandLineCollection
    {
        private RawOptionParser _parser = new RawOptionParser();
        private readonly Dictionary<string, string> _valuesKeyedByOptionName =
            new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        public int Count
        {
            get { return _valuesKeyedByOptionName.Count; }
        }

        public void Add(string optionNameAndValue)
        {
            var pair = _parser.SplitOptionNameAndValue(optionNameAndValue);
            if (pair.Key != null)
            {
                _valuesKeyedByOptionName.Add(pair.Key, pair.Value);
            }
        }

        public string GetValue(Option option)
        {
            string value = GetValue(option.ShortName);
            if (value == null)
            {
                value = GetValue(option.LongName);
            }
            return value;
        }

        public string GetValue(string optionName)
        {
            if (optionName != null && _valuesKeyedByOptionName.ContainsKey(optionName))
            {
                return _valuesKeyedByOptionName[optionName];
            }
            return null;
        }

        public bool Contains(string optionName)
        {
            return optionName != null && _valuesKeyedByOptionName.ContainsKey(optionName);
        }

        public bool Contains(Option option)
        {
            return Contains(option.ShortName) || Contains(option.LongName);
        }
    }
}
