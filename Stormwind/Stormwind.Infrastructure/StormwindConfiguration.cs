using System;
using System.Configuration;
using Stormwind.Infrastructure.Data;

namespace Stormwind.Infrastructure
{
	/// <summary>
	/// Stormwind custom app.config configuration section.
	/// </summary>
	[Serializable]
	public class StormwindConfiguration
	{
		public DatabaseSettings DatabaseSettings { get; set; }
		public bool DevMode { get; set; }

		/// <summary>
		/// Gets the current configuration from the app.config.
		/// </summary>
		public static StormwindConfiguration GetConfiguration()
		{
			return (StormwindConfiguration)ConfigurationManager.GetSection("StormwindConfiguration");
		}
	}
}
