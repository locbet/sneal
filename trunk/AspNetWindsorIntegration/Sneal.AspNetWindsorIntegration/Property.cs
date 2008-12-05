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
using System.Reflection;

namespace Sneal.AspNetWindsorIntegration
{
    /// <summary>
    /// Wraps a .NET PropertyInfo instance
    /// </summary>
    public class Property
    {
        private PropertyInfo propertyInfo;

        public Property(PropertyInfo propertyInfo)
        {
            this.propertyInfo = propertyInfo;
        }

        public virtual void SetValue(object instance, object dependency)
        {
            if (dependency == null)
                return;

            TrySetValue(instance, dependency);
        }

        private void TrySetValue(object instance, object dependency)
        {
            try
            {
                propertyInfo.SetValue(instance, dependency, null);
            }
            catch (Exception ex)
            {
                string msg = string.Format(
                    "Could not set dependency via property setter: {0}, on type {1}",
                    propertyInfo.Name, propertyInfo.GetType());

                throw new ApplicationException(msg, ex);
            }  
        }

        public virtual bool CanWrite
        {
            get { return propertyInfo.CanWrite; }
        }

        public virtual bool IsRequiredDependency
        {
            get
            {
                return propertyInfo.GetCustomAttributes(typeof(RequiredDependencyAttribute), true).Length > 0;
            }
        }

        public virtual bool IsOptionalDependency
        {
            get
            {
                return propertyInfo.GetCustomAttributes(typeof(OptionalDependencyAttribute), true).Length > 0;
            }
        }

        public PropertyInfo PropertyInfo
        {
            get { return propertyInfo; }
            set { propertyInfo = value; }
        }
    }
}
