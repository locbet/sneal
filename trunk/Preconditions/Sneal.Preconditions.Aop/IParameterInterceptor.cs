using System.Reflection;

namespace Sneal.Preconditions.Aop
{
    /// <summary>
    /// Defines an interceptor for a particular method parameter based on the
    /// parameter reflection info.
    /// </summary>
    public interface IParameterInterceptor
    {
        /// <summary>
        /// Returns whether this parameter interceptor is capable of intercepting
        /// and handling the parameter invocation.
        /// </summary>
        /// <param name="parameterInfo">The paramter info of the intercepted parameter</param>
        /// <returns><c>true</c> if this interceptor can handle the parameter, otherwise <c>false</c>.</returns>
        bool Accept(ParameterInfo parameterInfo);

        /// <summary>
        /// Handles the parameter interception.
        /// </summary>
        /// <param name="parameterInfo">The parameter info</param>
        /// <param name="parameterValue">The raw value of the parameter.</param>
        void Intercept(ParameterInfo parameterInfo, object parameterValue);
    }
}
