using System;
using System.Reflection;
using Castle.Core.Interceptor;
using Castle.MicroKernel;

namespace Sneal.Preconditions.Aop
{
    /// <summary>
    /// Interceptor used to verify method arguments are valid before the
    /// method is invoked.
    /// </summary>
    public class ParameterInterceptor : IInterceptor
    {
        private readonly IKernel kernel;

        public ParameterInterceptor(IKernel kernel)
        {
            this.kernel = kernel;
        }

        public void Intercept(IInvocation invocation)
        {
            InterceptParameters(invocation, invocation.Method.GetParameters());

            MethodInfo interfaceMethod = GetInterfaceMethod(invocation.Method);
            if (interfaceMethod != null)
            {
                InterceptParameters(invocation, interfaceMethod.GetParameters());
            }            

            invocation.Proceed();
        }

        /// <summary>
        /// Attempts to intercept each parameter.
        /// </summary>
        /// <param name="invocation">The method invocation</param>
        /// <param name="parameters">
        /// All the parameter reflection info for the method, either from the
        /// concrete type or an interface.
        /// </param>
        private void InterceptParameters(IInvocation invocation, ParameterInfo[] parameters)
        {
            for (int paramIdx = 0; paramIdx < parameters.Length; paramIdx++)
            {
                object paramValue = invocation.Arguments[paramIdx];

                ParameterInfo paramInfo = parameters[paramIdx];
                InterceptParameter(paramInfo, paramValue);
            }
        }

        /// <summary>
        /// Intercepts the parameter for eveyr registered <see cref="IParameterInterceptor"/>
        /// </summary>
        /// <param name="paramInfo">The reflection info for the parameter</param>
        /// <param name="paramValue">The raw parameter value</param>
        private void InterceptParameter(ParameterInfo paramInfo, object paramValue)
        {
            foreach (var paramInterceptor in kernel.ResolveAll<IParameterInterceptor>())
            {
                if (paramInterceptor.Accept(paramInfo))
                {
                    paramInterceptor.Intercept(paramInfo, paramValue);
                }                
            }
        }

        /// <summary>
        /// Utility method that grabs the method info of the base interface
        /// type if the concrete method is an interface implementation.
        /// </summary>
        /// <param name="concreteMethod">The concrete class's method info.</param>
        /// <returns>The interface method info if found, otherwise null.</returns>
        private static MethodInfo GetInterfaceMethod(MethodInfo concreteMethod)
        {
            Type concreteType = concreteMethod.ReflectedType;

            foreach (Type interfaceType in concreteType.GetInterfaces())
            {
                InterfaceMapping map = concreteType.GetInterfaceMap(interfaceType);

                // iterate through the MethodInfos
                for (int i = 0; i < map.TargetMethods.Length; i++)
                {
                    if (map.TargetMethods[i] == concreteMethod)
                    {
                        return map.InterfaceMethods[i];
                    }
                }
            }

            return null;
        }
    }
}
