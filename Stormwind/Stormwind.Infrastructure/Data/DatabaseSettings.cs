using System;
using System.Collections.Generic;
using NHibernate.Dialect;

namespace Stormwind.Infrastructure.Data
{
	[Serializable]
	public class DatabaseSettings
	{
		/// <summary>
		/// Creates settings that default to localhost SQL server using integrated auth.
		/// </summary>
		public DatabaseSettings()
		{
			Provider = "NHibernate.Connection.DriverConnectionProvider";
			DriverClass = "NHibernate.Driver.SqlClientDriver";
			Dialect = "NHibernate.Dialect.MsSql2005Dialect";
			ConnectionString = "Data Source=localhost;Initial Catalog=Stormwind;Integrated Security=SSPI;";
			ProxyFactory = "NHibernate.ByteCode.Castle.ProxyFactoryFactory, NHibernate.ByteCode.Castle";
			UseOuterJoin = true;
		}

		public string Provider { get; set; }
        public string DriverClass { get; set; }
        public string Dialect { get; set; }
        public string ConnectionString { get; set; }
        public string ProxyFactory { get; set; }
		public bool UseOuterJoin { get; set; }
		public bool ShowSql { get; set; }
		public bool CreateDatabase { get; set; }

		/// <summary>
		/// Converts this instance to key value pairs specifically for passing
		/// to NHibernate configuration.
		/// </summary>
		public IDictionary<string, string> GetAsNHiberateProperties()
		{
			var properties = new Dictionary<string, string>
			{
				{"connection.provider", Provider},
				{"connection.driver_class", DriverClass},
				{"dialect", Dialect},
				{"use_outer_join", UseOuterJoin.ToString()},
				{"connection.connection_string", ConnectionString},
				{"show_sql", ShowSql.ToString()},
				{"proxyfactory.factory_class", ProxyFactory}
			};
			return properties;
		}
	}
}
