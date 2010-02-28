using System;
using System.Configuration;
using System.Xml;
using System.Xml.Serialization;

namespace Stormwind.Infrastructure
{
	/// <summary>
	/// Deserializes the StormwindConfiguration from the app.config.
	/// </summary>
	public class StormwindSectionHandler : IConfigurationSectionHandler
	{
		public object Create(object parent, object configContext, XmlNode section)
		{
			try
			{
				return CreateXmlSerializer().Deserialize(new XmlNodeReader(section));
			}
			catch (Exception ex)
			{
				throw new ConfigurationErrorsException(
					"Could not load the Stormwind configuration section from the web.config or app.config", ex);
			}
		}

		private static XmlSerializer CreateXmlSerializer()
		{
			return new XmlSerializer(typeof(StormwindConfiguration));
		}
	}
}
