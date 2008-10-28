using Castle.Core;
using Castle.MicroKernel;
using Castle.MicroKernel.ModelBuilder;

namespace Sneal.Preconditions.Aop
{
    /// <summary>
    /// Adds a ParameterInterceptor to each component in the container.
    /// </summary>
    public class ParameterInterceptorContributor : IContributeComponentModelConstruction
    {
        public void ProcessModel(IKernel kernel, ComponentModel model)
        {
            // add to all components for now...
            model.Interceptors.Add(new InterceptorReference(typeof(ParameterInterceptor)));
        }
    }
}
