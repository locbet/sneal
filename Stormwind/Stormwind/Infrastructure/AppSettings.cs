using System.Configuration;

namespace Stormwind.Infrastructure
{
    /// <summary>
    /// Statically typed application settings. This class tries to provide common
    /// default values, but all values can be overridden via the app.config or
    /// web.config.
    /// </summary>
    public class AppSettings
    {
        private string _rootContentPath = "~/Content";
        private string _connectionString;

        /// <summary>
        /// The root URL to the site's static content.  Defaults to ~/Content.
        /// </summary>
        public string RootContentPath
        {
            get
            {
                return ConfigurationManager.AppSettings["RootContentPath"] ?? _rootContentPath;
            }
            set
            {
                _rootContentPath = value;
            }
        }

        public string ConnectionString
        {
            get
            {
                return ConfigurationManager.AppSettings["ConnectionString"] ?? _connectionString;
            }
            set
            {
                _connectionString = value;
            }
        }
    }
}