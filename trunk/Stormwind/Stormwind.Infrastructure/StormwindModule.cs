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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Autofac;
using Autofac.Builder;
using Autofac.Modules;
using AutofacContrib.Startable;
using Stormwind.Core.Security;
using Stormwind.Infrastructure.Data;
using Stormwind.Infrastructure.Data.Repositories;
using Stormwind.Infrastructure.Security;
using Module = Autofac.Builder.Module;

namespace Stormwind.Infrastructure
{
	/// <summary>
	/// The Stormwind Autofac registration module.
	/// </summary>
	public class StormwindModule : Module
	{
		protected override void Load(ContainerBuilder builder)
		{
			RegisterDependantModules(builder);
			RegisterConfiguration(builder);
			RegisterUnitOfWork(builder);
			RegisterRepositories(builder);
			RegisterSecurityServices(builder);
		}

		private static void RegisterDependantModules(ContainerBuilder builder)
		{
			// Enables the ability to resolve instances as IEnumerable<T>
			builder.RegisterModule(new ImplicitCollectionSupportModule());

			// Enables startable services
			builder.RegisterModule(new StartableModule<IStartable>(s => s.Start()));
		}

		private static void RegisterConfiguration(ContainerBuilder builder)
		{
			builder.Register(StormwindConfiguration.GetConfiguration())
				.As<StormwindConfiguration>()
				.SingletonScoped();
			builder.Register(c => c.Resolve<StormwindConfiguration>().DatabaseSettings)
				.As<DatabaseSettings>()
				.SingletonScoped();
		}

		private static void RegisterUnitOfWork(ContainerBuilder builder)
		{
			builder.Register<SessionProvider>()
				.As<ISessionProvider>()
				.OnActivated(StartStarable)
				.SingletonScoped();
			builder.Register<UnitOfWork>()
				.As<IUnitOfWorkImplementor>()
				.As<IUnitOfWork>()
				.OnActivated(StartStarable)
				.WithProperties(new NamedPropertyParameter("CommitMode", CommitMode.Implicit))
				.ContainerScoped();
		}

		private static void StartStarable(object source, ActivatedEventArgs e)
		{
			((IStartable)e.Instance).Start();
		}

		private static void RegisterRepositories(ContainerBuilder containerBuilder)
		{
			// register each repository impl
			foreach (Type repository in GetRepositoryImplementations())
			{
				// get the implementation's entity specific interface
				// assume its the first interface
				Type repositoryInterface =
					repository.GetInterfaces().Where(i => !i.IsGenericType).First();

				containerBuilder.Register(repository)
					.As(repositoryInterface)
					.ContainerScoped();
			}
		}

		private static IEnumerable<Type> GetRepositoryImplementations()
		{
			// get all repository implementations from infrastructure assembly
			return from type in Assembly.GetAssembly(typeof (RepositoryBase<>)).GetTypes()
			       where !type.IsAbstract && type.Namespace.EndsWith(@"Repositories")
			       select type;
		}

		private static void RegisterSecurityServices(ContainerBuilder containerBuilder)
		{
			containerBuilder.Register<AuthenticationService>()
				.ContainerScoped();
			containerBuilder.Register<UserRegistrationService>()
				.ContainerScoped();
			containerBuilder.Register<Md5PasswordHashService>()
				.As<IPasswordHashService>()
				.SingletonScoped();
		}
	}
}