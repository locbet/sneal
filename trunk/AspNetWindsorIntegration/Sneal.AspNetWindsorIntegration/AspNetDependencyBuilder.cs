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
using System.Web.UI;
using Castle.MicroKernel;
using Castle.Windsor;

namespace Sneal.AspNetWindsorIntegration
{
    /// <summary>
    /// Builds up a page instance, injecting any required dependencies.
    /// </summary>
    /// <remarks>This class is not thread safe.</remarks>
    public class AspNetDependencyBuilder
    {
        private readonly IWindsorContainer container;
        private object instance;
        private Type instanceType;

        public AspNetDependencyBuilder(IWindsorContainer container)
        {
            if (container == null) throw new ArgumentNullException("container");
            this.container = container;
        }

        public virtual void BuildUp(object instanceToBuildUp)
        {
            if (instanceToBuildUp == null) throw new ArgumentNullException("instanceToBuildUp");
            instance = instanceToBuildUp;
            instanceType = instanceToBuildUp.GetType();

            // optimization, skip reflecting on the code generated derived page type
            if (instance is Page)
            {
                instanceType = instanceType.BaseType;
            }

            BuildUpInternal();
        }

        protected virtual void BuildUpInternal()
        {
            foreach (Property property in PropertyFinder().PropertiesToSet())
            {
                object dependency = TryResolveDependency(property.PropertyInfo);
                property.SetValue(instance, dependency);
            }
        }

        protected virtual object TryResolveDependency(PropertyInfo property)
        {
            try
            {
                return container.Resolve(property.PropertyType);
            }
            catch (ComponentNotFoundException)
            {
                if (PageInjectionBehavior == For.ExplicitProperties)
                {
                    throw;
                }
            }

            return null;
        }

        private For PageInjectionBehavior
        {
            get
            {
                var attributes = instanceType.GetCustomAttributes(typeof(UsesInjectionAttribute), true);
                if (attributes.Length > 0)
                {
                    return ((UsesInjectionAttribute)attributes[0]).Behavior;                
                }
                return For.None;
            }
        }

        private IPropertyFinder PropertyFinder()
        {
            return PropertyFinderFactory.Create(PageInjectionBehavior, instanceType);
        }
    }
}
