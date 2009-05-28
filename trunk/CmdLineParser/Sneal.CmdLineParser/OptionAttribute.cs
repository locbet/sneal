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

namespace Sneal.CmdLineParser
{
    /// <summary>
    /// Attribute to apply to a class property to be set from the command
    /// line.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class OptionAttribute : Attribute
    {
        private string _description;
        private string _longName;
        private string _shortName;
        private bool _required;

        public string ShortName
        {
            get { return _shortName; }
            set { _shortName = value; }
        }

        public string LongName
        {
            get { return _longName; }
            set { _longName = value; }
        }

        public string Description
        {
            get { return _description; }
            set { _description = value; }
        }

        public bool Required
        {
            get { return _required; }
            set { _required = value; }
        }
    }
}
