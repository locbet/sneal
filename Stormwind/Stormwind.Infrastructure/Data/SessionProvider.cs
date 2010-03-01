using System.Collections.Generic;
using System.Data;
using System.Runtime.CompilerServices;
using FluentNHibernate.Automapping;
using FluentNHibernate.Cfg;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Dialect;
using Stormwind.Core.Models;

namespace Stormwind.Infrastructure.Data
{
	public class SessionProvider : IStartable, ISessionProvider
	{
		private readonly DatabaseSettings _databaseSettings;
		private Configuration _configuration;
		private ISessionFactory _sessionFactory;
		private bool _isStarted;

		public SessionProvider(DatabaseSettings databaseSettings)
		{
			_databaseSettings = databaseSettings;
		}

		public void Start()
		{
			if (!_isStarted)
			{
				CreateSessionFactory();
			}
			_isStarted = true;
		}

		public ISession CreateSession()
		{
			ISession session = _sessionFactory.OpenSession();
			session.FlushMode = FlushMode.Never;
			return session;
		}

		public void BuildSchema()
		{
            ISession session = CreateSession();
            IDbConnection connection = session.Connection;
			Dialect dialect = Dialect.GetDialect(_databaseSettings.GetAsNHiberateProperties());

			string[] drops = _configuration.GenerateDropSchemaScript(dialect);
            ExecuteScripts(drops, connection);
 
            string[] scripts = _configuration.GenerateSchemaCreationScript(dialect);
			ExecuteScripts(scripts, connection);
		}

		private void CreateSessionFactory()
		{
			Configuration configuration = CreateConfiguration();
			_sessionFactory = configuration.BuildSessionFactory();
		}

		[MethodImpl(MethodImplOptions.Synchronized)]
		private Configuration CreateConfiguration()
		{
			return _configuration = Fluently.Configure()
				.Database(new StormwindPersistenceConfigurer(_databaseSettings))
				.Mappings(m => m.AutoMappings.Add(GetAutoPersistenceModel()))
				.BuildConfiguration();
		}

		private static AutoPersistenceModel GetAutoPersistenceModel()
		{
			return AutoMap.AssemblyOf<User>()
				//.IgnoreBase(typeof (Entity<>))
				.Where(t => t.Namespace == @"Stormwind.Core.Models");
		}

		private static void ExecuteScripts(IEnumerable<string> scripts, IDbConnection connection)
		{
			foreach (string script in scripts)
			{
				IDbCommand command = connection.CreateCommand();
				command.CommandText = script;
				command.ExecuteNonQuery();
			}
		}
	}
}
