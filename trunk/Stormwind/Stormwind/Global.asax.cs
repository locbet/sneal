using System;
using System.Web;
using Autofac.Integration.Web;
using Stormwind.Infrastructure;

namespace Stormwind
{
    public class MvcApplication : HttpApplication, IContainerProviderAccessor
    {
        private static readonly Bootstrap Bootstrap = new Bootstrap(new AppSettings());
        private static readonly object BootstrapLock = new object();

        protected void Application_Start()
        {
            lock (BootstrapLock)
            {
                Bootstrap
                    .DependencyInjectionContainer()
                    .MvcRoutes()
                    .NHibernate()
                    .Go();
            }
        }

        protected void Application_EndRequest(object sender, EventArgs e)
        {
            ContainerProvider.DisposeRequestContainer();
        }

        public IContainerProvider ContainerProvider
        {
            get { return Bootstrap.ContainerProvider; }
        }
    }
}