using System.IO;
using System.Reflection;
using log4net;
using log4net.Config;
using log4net.Repository.Hierarchy;

namespace Sneal.SqlMigration.Console
{
    internal static class Log4NetConfigurator
    {
        public static void ConfigureLog4Net(string configFilePath)
        {
            if (string.IsNullOrEmpty(configFilePath))
            {
                DefaultLog4NetConfiguration();
                return;
            }

            if (!File.Exists(configFilePath))
                throw new SqlMigrationException("Could not find the log4net configuration file.");

            using (StreamReader reader = new StreamReader(configFilePath))
            {
                XmlConfigurator.Configure(reader.BaseStream);
            }
        }

        public static void SetLevel(ILog log, string levelName)
        {
            Logger l = (Logger) log.Logger;
            l.Level = l.Hierarchy.LevelMap[levelName];
        }

        private static void DefaultLog4NetConfiguration()
        {
            using (Stream s = Assembly.GetExecutingAssembly().GetManifestResourceStream(
                "Sneal.SqlMigration.Console.log4net.config"))
            {
                using (StreamReader reader = new StreamReader(s))
                {
                    XmlConfigurator.Configure(reader.BaseStream);
                }
            }
        }
    }
}