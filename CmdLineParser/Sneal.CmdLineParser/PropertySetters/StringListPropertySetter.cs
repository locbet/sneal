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
using System.Reflection;

namespace Sneal.CmdLineParser.PropertySetters
{
    /// <summary>
    /// Sets a property of type IList&gt;string&gt;
    /// </summary>
    public class StringListPropertySetter : IPropertySetter
    {
        public void SetPropertyValue(Option option, PropertyInfo propertyInfo, object instanceToSet, string rawValue)
        {
            var values = new List<string>();
            string val = rawValue == null ? rawValue : rawValue.Trim();
            if (val != null)
            {
                foreach (var s in val.Split(new[] { ';', ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    values.Add(s);
                }
            }

            propertyInfo.SetValue(instanceToSet, values, null);
        }

        public Type SupportedType
        {
            get { return typeof(IList<string>); }
        }
    }
}
