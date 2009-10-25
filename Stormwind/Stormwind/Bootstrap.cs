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
            var builder = new ContainerBuilder();
            builder.RegisterModule(new AutofacControllerModule(Assembly.GetExecutingAssembly()));
            ContainerProvider = new ContainerProvider(builder.Build());
            ControllerBuilder.Current.SetControllerFactory(new AutofacControllerFactory(ContainerProvider));

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
    }
}