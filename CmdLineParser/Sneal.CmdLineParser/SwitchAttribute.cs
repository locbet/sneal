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
    [AttributeUsage(AttributeTargets.Property)]
    public class SwitchAttribute : Attribute
    {
        private string description;
        private string name;
        private bool required;

        public SwitchAttribute(string name, string description, bool required)
        {
            this.description = description;
            this.name = name;
            this.required = required;
        }

        public SwitchAttribute(string name, string description)
        {
            this.name = name;
            this.description = description;
        }

        public SwitchAttribute(string name)
        {
            this.name = name;
        }

        public SwitchAttribute()
        {
        }

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public string Description
        {
            get { return description; }
            set { description = value; }
        }

        public bool Required
        {
            get { return required; }
            set { required = value; }
        }
    }
}
