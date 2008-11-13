using System;
using Castle.MicroKernel.Registration;
using Castle.Windsor;

namespace Sneal.AspNetWindsorIntegration.WebSample
{
    public class Global : System.Web.HttpApplication, IContainerAccessor
    {
        private readonly IWindsorContainer container = new WindsorContainer();

        public Global()
        {
            container.Register(
                Component.For<ICustomerRepository>()
                    .ImplementedBy<InMemoryCustomerRepository>());
        }

        protected void Application_Start(object sender, EventArgs e)
        {

        }

        protected void Session_Start(object sender, EventArgs e)
        {

        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {

        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {

        }

        protected void Application_Error(object sender, EventArgs e)
        {

        }

        protected void Session_End(object sender, EventArgs e)
        {

        }

        protected void Application_End(object sender, EventArgs e)
        {

        }

        public IWindsorContainer Container
        {
            get { return container; }
        }
    }
}