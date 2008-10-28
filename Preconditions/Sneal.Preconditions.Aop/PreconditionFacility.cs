using Castle.Core.Configuration;
using Castle.MicroKernel;
using Castle.MicroKernel.Registration;
using Sneal.Preconditions.Aop.Interceptors;

namespace Sneal.Preconditions.Aop
{
    /// <summary>
    /// Facility used for intercepting specific parameters on a method
    /// invocation.  Each <see cref="IParameterInterceptor"/> registered
    /// with the container is tested whether it needs to intercept each
    /// paramter, if true its intercept  method is called.  By default 
    /// this facilty registers <see cref="NotNullParameterInterceptor"/>
    /// and <see cref="NotNullOrEmptyParameterInterceptor"/>.
    /// </summary>
    public class PreconditionFacility : IFacility
    {
        public void Init(IKernel kernel, IConfiguration facilityConfig)
        {
            kernel.Register(
                Component.For<IParameterInterceptor>()
                    .ImplementedBy<NotNullParameterInterceptor>(),
                Component.For<IParameterInterceptor>()
                    .ImplementedBy<NotNullOrEmptyParameterInterceptor>());

            kernel.AddComponent("preconditions.interceptor", typeof(ParameterInterceptor));
            kernel.ComponentModelBuilder.AddContributor(new ParameterInterceptorContributor());
        }

        public void Terminate()
        {
            // no-op
        }
    }
}
