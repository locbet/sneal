using System;
using System.Reflection;

namespace Sneal.Preconditions.Aop.Interceptors
{
    /// <summary>
    /// Intercepts a parameter marked with a NotNull attribute and checks
    /// that the value is non-null.
    /// </summary>
    public class NotNullParameterInterceptor : BaseAttributeParameterInterceptor
    {
        public override bool Accept(Attribute attribute)
        {
            return attribute.GetType().Name == "NotNullAttribute";
        }

        /// <summary>
        /// Throws an ArgumentNullException if the parameter value is null.  This
        /// method should not be called unless Accept returns <c>true</c>.
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown if the parameterValue is null.</exception>
        /// <param name="parameterInfo">The parameter reflection info.</param>
        /// <param name="parameterValue">The raw parameter value.</param>
        public override void Intercept(ParameterInfo parameterInfo, object parameterValue)
        {
            new Precondition<object>(parameterValue, parameterInfo.Name).IsNull();
        }
    }
}