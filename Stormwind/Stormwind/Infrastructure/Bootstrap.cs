using System;
using System.Collections.Generic;
using System.Reflection;
using System.Web.Mvc;
using System.Web.Routing;
using Autofac.Builder;
using Autofac.Integration.Web;
using Autofac.Integration.Web.Mvc;
using AutofacContrib.CommonServiceLocator;
using Microsoft.Practices.ServiceLocation;
using NHibernate;

namespace Stormwind.Infrastructure
{
    /// <summary>
    /// Bootstraps and initializes Stormwind for operation.  This should only need to
    /// called once per appdomain.  Generally all methods should be called unless for
    /// testing purposes.
    /// </summary>
    public class Bootstrap
    {
        private readonly AppSettings _appSettings;
        private readonly Queue<Action> _containerDependantCommands = new Queue<Action>();
        private readonly NHibernateConfiguration _nhConfiguration = new NHibernateConfiguration();
        private ContainerBuilder _containerBuilder = new ContainerBuilder();

        public Bootstrap(AppSettings appSettings)
        {
            _appSettings = appSettings;
        }

        public IContainerProvider ContainerProvider { get; private set; }

        public Bootstrap DependencyInjectionContainer()
        {
            _containerBuilder = new ContainerBuilder();
            _containerBuilder.Register(_appSettings);
            return this;
        }

        public Bootstrap MvcRoutes()
        {
            RegisterMvcRoutes();
            return this;
        }

        public Bootstrap NHibernate()
        {
            _nhConfiguration.RegisterNHibernateSessionFactory(
                _containerBuilder, _appSettings.ConnectionString);
            return this;
        }

        public Bootstrap Schema()
        {
            if (_appSettings.DevMode)
            {
                _containerDependantCommands.Enqueue(() =>
                {
                    var session = ServiceLocator.Current.GetInstance<ISession>();
                    new NHibernateSchemaExport().ExportNHibernateSchema(session, _nhConfiguration.Configuration);
                });
            }
            return this;
        }

        public void Go()
        {
            CreateContainerAndSetServiceLocator();
            while (_containerDependantCommands.Count > 0)
            {
                _containerDependantCommands.Dequeue().Invoke();
            }            
        }

        private void CreateContainerAndSetServiceLocator()
        {
            ContainerProvider = new ContainerProvider(_containerBuilder.Build());
            ServiceLocator.SetLocatorProvider(() => new AutofacServiceLocator(ContainerProvider.ApplicationContainer));
        }

        private void RegisterMvcRoutes()
        {
            RouteCollection routes = RouteTable.Routes;
            if (routes != null)
            {
                routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
                routes.MapRoute(
                    "Default", // Route name
                    "{controller}/{action}/{id}", // URL with parameters
                    new {controller = "Home", action = "Index", id = ""} // Parameter defaults
                    );
            }
            _containerBuilder.RegisterModule(
                new AutofacControllerModule(Assembly.GetExecutingAssembly()));
        }
    }
}