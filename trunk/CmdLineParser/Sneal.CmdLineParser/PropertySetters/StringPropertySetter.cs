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
using System.Reflection;

namespace Sneal.CmdLineParser.PropertySetters
{
    public class StringPropertySetter : IPropertySetter
    {
        public void SetPropertyValue(Option option, PropertyInfo propertyInfo, object instanceToSet, string rawValue)
        {
            string val = rawValue == null ? rawValue : rawValue.Trim();
            propertyInfo.SetValue(instanceToSet, val, null);
        }

        public Type SupportedType
        {
            get { return typeof(string); }
        }

        public bool SupportsType(Type type)
        {
            return type == SupportedType;
        }
    }
}