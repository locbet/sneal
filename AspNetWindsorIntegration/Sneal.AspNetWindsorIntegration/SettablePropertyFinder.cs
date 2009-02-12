#region license
// Copyright 2008 Shawn Neal (sneal@sneal.net)
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

namespace Sneal.AspNetWindsorIntegration
{
    public class SettablePropertyFinder : IPropertyFinder
    {
        protected Type instanceType;

        public SettablePropertyFinder(Type instanceType)
        {
            this.instanceType = instanceType;
        }

        public virtual IEnumerable<Property> PropertiesToSet()
        {
            return GetPublicSetterPropertiesForType(instanceType);
        }

        protected virtual List<Property> GetPublicSetterPropertiesForType(Type curType)
        {
            var setterProperties = new List<Property>();
            if (TypeIsSettable(curType))
            {
                setterProperties.AddRange(GetAllProperties(curType).FindAll(ShouldIncludeProperty));
                setterProperties.AddRange(GetPublicSetterPropertiesForType(curType.BaseType));
            }
            return setterProperties;
        }

        private static bool TypeIsSettable(Type currentType)
        {
            if (currentType == null)
            {
                return false;
            }
            return !IsSystemNamespace(currentType.Namespace);
        }

        private static bool IsSystemNamespace(string @namespace)
        {
            if (string.IsNullOrEmpty(@namespace))
            {
                return false;
            }
            return @namespace.StartsWith("System");
        }

        private static List<Property> GetAllProperties(Type curType)
        {
            var allProperties = curType.GetProperties(
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly);
            return AdaptPropertyInfoToProperty(allProperties);
        }

        private static List<Property> AdaptPropertyInfoToProperty(IEnumerable<PropertyInfo> propertyInfos)
        {
            List<Property> properties = new List<Property>();
            foreach (PropertyInfo propInfo in propertyInfos)
            {
                properties.Add(new Property(propInfo));
            }
            return properties;
        }

        protected virtual bool ShouldIncludeProperty(Property property)
        {
            return property.CanWrite;
        }

        public Type InstanceType
        {
            get { return instanceType; }
            set { instanceType = value; }
        }
    }
}
