#region license
// Copyright 2010 Shawn Neal (sneal@sneal.net)
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion

using System.Net;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Web.Mvc;
using System.Web.Routing;
using Autofac;
using Autofac.Builder;
using Autofac.Integration.Web;
using Autofac.Integration.Web.Mvc;
using AutofacContrib.Startable;
using Stormwind.Core.Security;
using Stormwind.Infrastructure.Data;

namespace Stormwind.Infrastructure
{
    /// <summary>
    /// Bootstraps and initializes Stormwind for operation.  This should only need to
    /// called once per appdomain.  Generally all methods should be called unless for
    /// testing purposes.
    /// </summary>
    public class Bootstrap
    {
		public bool IsInitialized { get; private set; }
        public IContainerProvider ContainerProvider { get; private set; }

		[MethodImpl(MethodImplOptions.Synchronized)]
        public void StormwindApplication()
        {
			if (!IsInitialized)
			{
				Initialize();
				IsInitialized = true;
			}
        }

    	private void Initialize()
    	{
    		CreateContainer();
    		StartApplicationServices();
    		CreateSchema();
    		ConfigureMvc();
    	}

    	private IContainer ApplicationContainer
		{
			get { return ContainerProvider.ApplicationContainer; }
		}

    	private void CreateContainer()
    	{
    		var containerBuilder = new ContainerBuilder();
    		containerBuilder.RegisterModule(new StormwindModule());
    		containerBuilder.RegisterModule(new AutofacControllerModule(Assembly.GetAssembly(GetType())));
    		ContainerProvider = new ContainerProvider(containerBuilder.Build());
    	}

    	private void StartApplicationServices()
    	{
    		ApplicationContainer.Resolve<IStarter>().Start();
    	}

		private void ConfigureMvc()
    	{
			ControllerBuilder.Current.SetControllerFactory(
				new AutofacControllerFactory(ContainerProvider));

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
    	}

    	private void CreateSchema()
        {
			using (var context = ApplicationContainer.CreateInnerContainer())
			{
				var dbSettings = context.Resolve<DatabaseSettings>();
				if (dbSettings.CreateDatabase)
				{
					var sessionProvider = context.Resolve<ISessionProvider>();
					sessionProvider.BuildSchema();

					string adminEmail = CreateAdminEmail();
					var userRegistrationService = context.Resolve<UserRegistrationService>();
					userRegistrationService.RegisterNewUser(adminEmail, "password");
				}
			}
        }

		private static string CreateAdminEmail()
		{
			return @"admin@" + Dns.GetHostEntry("localhost").HostName.ToLowerInvariant();
		}
    }
}