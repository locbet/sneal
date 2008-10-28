using System;
using System.Reflection;

namespace Sneal.Preconditions.Aop.Interceptors
{
    /// <summary>
    /// Intercepts a parameter marked with a NotNullOrEmpty attribute and checks
    /// that the value is non-null and non-empty.
    /// </summary>
    public class NotNullOrEmptyParameterInterceptor : BaseAttributeParameterInterceptor
    {
        public override bool Accept(Attribute attribute)
        {
            return attribute.GetType().Name == "NotNullOrEmptyAttribute";
        }

        /// <summary>
        /// Throws an exception if the parameter value is null or empty.  This
        /// method should not be called unless Accept returns <c>true</c>.
        /// </summary>
        /// <exception cref="ArgumentException">Thrown if the parameterValue is empty.</exception>
        /// <exception cref="ArgumentNullException">Thrown if the parameterValue is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown if the parameter is not a string or implicitly convertable to a string.
        /// </exception>
        /// <param name="parameterInfo">The parameter reflection info.</param>
        /// <param name="parameterValue">The raw parameter value.</param>
        public override void Intercept(ParameterInfo parameterInfo, object parameterValue)
        {
            var value = parameterValue as string;
            if (value == null)
            {
                throw new ArgumentOutOfRangeException(
                    parameterInfo.Name,
                    "The NotNullOrEmpty attribute can only be applied to arguments of type string");
            }

            new StringPrecondition(value, parameterInfo.Name).IsNullOrEmpty();
        }
    }
}
