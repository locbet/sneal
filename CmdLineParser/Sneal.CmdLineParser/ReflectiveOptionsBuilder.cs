#region license
// Copyright 2010 Shawn Neal (neal.shawn@gmail.com)
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
using System.Collections.Generic;
using Sneal.CmdLineParser.PropertySetters;

namespace Sneal.CmdLineParser
{
	/// <summary>
	/// Builds a list of options up using reflection. Use this clas if you
	/// wish to define your options on a DTO class with Option attributes.
	/// </summary>
	public class ReflectiveOptionsBuilder
	{
		private readonly PropertySetterRegistry _propertySetterRegistry;
		
		public ReflectiveOptionsBuilder(PropertySetterRegistry propertySetterRegistry)
		{
			_propertySetterRegistry = propertySetterRegistry;
		}
		
		/// <summary>
		/// Reflectively builds a list of options from the specified TOption type's
		/// properties tagged with an <see cref="OptionAttribute"/>
		/// </summary>
		/// <returns>
		/// A new <see cref="IList"/> of <see cref="Option"/>
		/// </returns>
		public IList<Option> BuildOptions<TOption>()
		{
            return BuildOptions(typeof(TOption));
        }		
		
		/// <summary>
		/// Reflectively builds a list of options from the specified optionType's
		/// properties tagged with an <see cref="OptionAttribute"/>
		/// </summary>
		/// <returns>
		/// A new <see cref="IList"/> of <see cref="Option"/>
		/// </returns>		
		public IList<Option> BuildOptions(Type optionType)
		{
            var options = new List<Option>();
            foreach (PropertyInfo prop in optionType.GetProperties())
            {
				if (HasOptionAttribute(prop))
				{
					options.Add(CreateOptionFromProperty(prop));
				}
            }
			return options;			
		}
		
		private bool HasOptionAttribute(PropertyInfo property)
		{
			return GetOptionAttribute(property) != null;
		}
		
		private OptionAttribute GetOptionAttribute(PropertyInfo property)
		{
			object[] attrs = property.GetCustomAttributes(typeof(OptionAttribute), true);
			if (attrs != null && attrs.Length == 1)
			{
				return attrs[0] as OptionAttribute;	
			}
			return null;
		}
		
		private Option CreateOptionFromProperty(PropertyInfo property)
		{
            OptionAttribute optionAttribute = GetOptionAttribute(property);
            IPropertySetter propertySetter = _propertySetterRegistry.GetPropertySetter(property.PropertyType);
            return Option.Create(optionAttribute, property, propertySetter);	
		}
	}
}
