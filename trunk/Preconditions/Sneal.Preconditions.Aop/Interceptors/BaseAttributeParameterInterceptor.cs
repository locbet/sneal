using System;
using System.Reflection;
using Castle.Core.Interceptor;

namespace Sneal.Preconditions.Aop.Interceptors
{
    /// <summary>
    /// Base abstract class for parameter interceptors that intercept an argument
    /// base on the attributes.
    /// </summary>
    public abstract class BaseAttributeParameterInterceptor : IParameterInterceptor
    {
        /// <summary>
        /// Finds any attributes on the parameter declaration and calls the
        /// derrived classes Accept(Attribute) method.
        /// </summary>
        public virtual bool Accept(ParameterInfo parameterInfo)
        {
            object[] rawAttrs = parameterInfo.GetCustomAttributes(true);
            if (rawAttrs != null)
            {
                foreach (object rawAttr in rawAttrs)
                {
                    if (Accept((Attribute)rawAttr))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// This should be implemented in a derrived class.
        /// </summary>
        /// <param name="attribute">The parameter attribute to test.</param>
        /// <returns>
        /// <c>true</c> if the interceptor should intercept based off the attribute
        /// </returns>
        public abstract bool Accept(Attribute attribute);

        /// <summary>
        /// This should be implemented in a derrived class.
        /// </summary>
        public abstract void Intercept(ParameterInfo parameterInfo, object parameterValue);
    }
}
