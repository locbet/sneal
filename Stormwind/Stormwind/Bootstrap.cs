using System;
using System.Web.Mvc;
using System.Web.Routing;
using Autofac;
using Autofac.Builder;
using System.Reflection;
using Autofac.Integration.Web;
using Autofac.Integration.Web.Mvc;
using AutofacContrib.CommonServiceLocator;
using Microsoft.Practices.ServiceLocation;

namespace Stormwind
{
    /// <summary>
    /// Bootstraps and initializes Stormwind for operation.  This should only need to
    /// called once per appdomain.  Generally all methods should be called unless for
    /// testing purposes.
    /// </summary>
    public class Bootstrap
    {
        public IContainerProvider ContainerProvider { get; set; }

        public Bootstrap DependencyInjectionContainer()
        {
            CreateContainer();
            SetCommonServiceLocator();
            SetMvcControllerFactory();

            return this;
        }

        public Bootstrap MvcRoutes()
        {
            var routes = RouteTable.Routes;
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.MapRoute(
                "Default",                                              // Route name
                "{controller}/{action}/{id}",                           // URL with parameters
                new { controller = "Home", action = "Index", id = "" }  // Parameter defaults
            );

            return this;
        }

        private void CreateContainer()
        {
            var builder = new ContainerBuilder();
            builder.RegisterModule(new AutofacControllerModule(Assembly.GetExecutingAssembly()));
            ContainerProvider = new ContainerProvider(builder.Build());
        }

        private void SetCommonServiceLocator()
        {
            ServiceLocator.SetLocatorProvider(() => new AutofacServiceLocator(ContainerProvider.ApplicationContainer));
        }

        private void SetMvcControllerFactory()
        {
            ControllerBuilder.Current.SetControllerFactory(new AutofacControllerFactory(ContainerProvider));
        }
    }
}