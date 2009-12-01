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

namespace Sneal.CmdLineParser.PropertySetters
{
    /// <summary>
    /// Registry of all available property setters.
    /// </summary>
    public class PropertySetterRegistry
    {
        private readonly List<IPropertySetter> _propertySetters = new List<IPropertySetter>();
        private static readonly IPropertySetter DefaultSetter = new NoOpPropertySetter();

        ///<summary>
        /// Gets the property setter cabable of handling the specified property
        /// type, or NoOpPropertySetter if there are no matches.
        ///</summary>
        ///<param name="propertyType"></param>
        ///<returns></returns>
        public IPropertySetter GetPropertySetter(Type propertyType)
        {
            foreach (var setter in _propertySetters)
            {
                if (setter.SupportsType(propertyType))
                {
                    return setter;
                }
            }
            return DefaultSetter;
        }

        /// <summary>
        /// Registers a property setter strategy.
        /// </summary>
        /// <param name="propertySetter">The setter instance.</param>
        public void RegisterPropertySetter(IPropertySetter propertySetter)
        {
            _propertySetters.Add(propertySetter);
        }
    }
}
