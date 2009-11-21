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
        private string _dbType = "MySql";
        private bool _devMode;

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

        /// <summary>
        /// The type of DB to use, can be: SqlServer or MySql.
        /// The connection string key in the app.config should match this value.
        /// </summary>
        public string DbType
        {
            get
            {
                return ConfigurationManager.AppSettings["DbType"] ?? _dbType;
            }
            set
            {
                _dbType = value;
            }
        }

        /// <summary>
        /// The Stormwind MySQL database connection string.
        /// </summary>
        public string ConnectionString
        {
            get
            {
                var connStr = ConfigurationManager.ConnectionStrings[DbType];
                if (connStr != null)
                {
                    return connStr.ConnectionString;
                }
                return _connectionString;
            }
            set
            {
                _connectionString = value;
            }
        }

        /// <summary>
        /// Is the site running in development mode? Defaults to false.
        /// </summary>
        public bool DevMode
        {
            get
            {
                string devMode = ConfigurationManager.AppSettings["DevMode"];
                if (!string.IsNullOrEmpty(devMode))
                {
                    _devMode = bool.Parse(devMode);
                }
                return _devMode;
            }
            set
            {
                _devMode = value;
            }
        }
    }
}