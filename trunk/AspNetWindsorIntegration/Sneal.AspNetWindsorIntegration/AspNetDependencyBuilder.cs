using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Web.UI;
using Castle.MicroKernel;
using Castle.Windsor;

namespace Sneal.AspNetWindsorIntegration
{
    public class AspNetDependencyBuilder
    {
        private readonly IWindsorContainer container;

        public AspNetDependencyBuilder(IWindsorContainer container)
        {
            if (container == null) throw new ArgumentNullException("container");
            this.container = container;
        }

        public virtual void BuildUp(object instanceToBuildUp)
        {
            if (instanceToBuildUp == null) throw new ArgumentNullException("instanceToBuildUp");
            Type instanceType = instanceToBuildUp.GetType();

            // optimization, skip reflecting on the code generated derived page type
            if (instanceToBuildUp is Page)
            {
                instanceType = instanceType.BaseType;
            }

            BuildUpInternal(instanceToBuildUp, instanceType);
        }

        protected virtual void BuildUpInternal(object instanceToBuildUp, Type instanceType)
        {
            foreach (PropertyInfo property in GetPublicSetterProperties(instanceType))
            {
                object dependency = TryResolveDependency(property);
                if (dependency != null)
                {
                    InjectDependency(property, dependency, instanceToBuildUp);
                }
            }

            // keep building the object further up the inheritence hierarchy
            // stop at built-in BCL types
            if (!instanceType.BaseType.Namespace.StartsWith("System"))
            {
                BuildUpInternal(instanceToBuildUp, instanceType.BaseType);
            }
        }

        protected virtual void InjectDependency<T>(PropertyInfo property, object dependency, T instanceToBuildUp)
        {
            Type instanceType = typeof(T);
            try
            {
                property.SetValue(instanceToBuildUp, dependency, null);
            }
            catch (Exception ex)
            {
                string msg = string.Format(
                    "Could not set dependency via property setter: {0}, on type {1}",
                    property.Name, instanceType);

                throw new ApplicationException(msg, ex);
            }
        }

        protected virtual object TryResolveDependency(PropertyInfo property)
        {
            try
            {
                return container.Resolve(property.PropertyType);
            }
            catch (ComponentNotFoundException ex)
            {
                string msg = string.Format(
                    "Found writable property {0} of type {1}, but could not resolve a dependency for it",
                    property.Name,
                    property.PropertyType);

                Debug.WriteLine(msg);
                Debug.Write(ex);
            }

            return null;
        }

        private static IEnumerable<PropertyInfo> GetPublicSetterProperties(Type instanceType)
        {
            var properties = instanceType.GetProperties(
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly);

            return Array.FindAll(properties, o => o.CanWrite);
        }
    }
}
