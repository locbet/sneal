using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Autofac.Integration.Web;
using Autofac;
using Autofac.Integration.Web.Mvc;
using System.Reflection;
using Stormwind.Infrastructure;

namespace Stormwind
{
    public class MvcApplication : HttpApplication, IContainerProviderAccessor
    {
        private static Bootstrap _bootstrap = new Bootstrap();

        protected void Application_Start()
        {
            lock (typeof(MvcApplication))
            {
                _bootstrap
                    .DependencyInjectionContainer()
                    .MvcRoutes()
                    .NHibernate();
            }
        }

        protected void Application_EndRequest(object sender, EventArgs e)
        {
            ContainerProvider.DisposeRequestContainer();
        }

        public IContainerProvider ContainerProvider
        {
            get { return _bootstrap.ContainerProvider; }
        }
    }
}