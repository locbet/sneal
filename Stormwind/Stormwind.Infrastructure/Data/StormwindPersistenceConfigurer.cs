using System;
using FluentNHibernate.Cfg.Db;
using NHibernate.Cfg;

namespace Stormwind.Infrastructure.Data
{
	public class StormwindPersistenceConfigurer : IPersistenceConfigurer
	{
		private readonly DatabaseSettings _dbSettings;

		public StormwindPersistenceConfigurer(DatabaseSettings dbSettings)
		{
			_dbSettings = dbSettings;
		}

		public Configuration ConfigureProperties(Configuration nhibernateConfig)
		{
			nhibernateConfig.SetProperties(_dbSettings.GetAsNHiberateProperties());
			if (IsMySqlDriver())
			{
				// http://stackoverflow.com/questions/1061128/mysql-hibernate-how-fix-the-error-column-reservedword-does-not-belong-to-ta
				nhibernateConfig.SetProperty("hbm2ddl.keywords", "none");
			}
			return nhibernateConfig;
		}

		public bool IsMySqlDriver()
		{
			return "NHibernate.Driver.MySqlDataDriver".Equals(
				_dbSettings.DriverClass, StringComparison.OrdinalIgnoreCase);
		}
	}
}
