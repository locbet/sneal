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

using System.Reflection;

namespace Sneal.CmdLineParser
{
    /// <summary>
    /// Represents a settable command line option.
    /// </summary>
    public class Option
    {
        /// <summary>
        /// The command line arg short switch name.
        /// </summary>
        public string ShortName { get; set; }

        /// <summary>
        /// The optional command line arg long switch name.
        /// </summary>
        public string LongName { get; set; }

        /// <summary>
        /// The command line arg help description.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Is the command line argument required?
        /// </summary>
        public bool IsRequired { get; set; }

        private PropertyInfo _propertyInfo;
        private IPropertySetter _propertySetter;

        /// <summary>
        /// Creates a new option instance from the specified attribute.
        /// </summary>
        public static Option Create(OptionAttribute attribute, PropertyInfo propertyInfo, IPropertySetter propertySetter)
        {
            return new Option
            {
                LongName = attribute.LongName,
                ShortName = attribute.ShortName,
                Description = attribute.Description,
                IsRequired = attribute.Required,
                _propertyInfo = propertyInfo,
                _propertySetter = propertySetter
            };
        }

        /// <summary>
        /// Returns the long name or the short depending on which is used.  If
        /// both are specified then the long name is returned.
        /// </summary>
        public string Name
        {
            get
            {
                return !string.IsNullOrEmpty(LongName) ? LongName : ShortName;
            }
        }

        /// <summary>
        /// Sets the option instance using the specified value.
        /// </summary>
        /// <param name="optionInstance">The option object</param>
        /// <param name="optionValue">The raw option value</param>
        public void SetValue(object optionInstance, string optionValue)
        {
            _propertySetter.SetPropertyValue(this, _propertyInfo, optionInstance, optionValue);            
        }
    }
}
