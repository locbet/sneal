using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;

namespace Stormwind.Infrastructure
{
    /// <summary>
    /// Statically typed application settings.
    /// </summary>
    public class AppSettings
    {
        /// <summary>
        /// The root URL to the site's static content.  Defaults to ~/Content.
        /// </summary>
        public string RootContentPath
        {
            get
            {
                return ConfigurationManager.AppSettings["RootContentPath"] ?? "~/Content";
            }
        }
    }
}