using System.IO;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace Sneal.JsUnitUtils.MsBuild
{
    /// <summary>
    /// Restores a web.config.bak to web.config in the specified directory.
    /// </summary>
    public class RestoreWebConfigTask : Task
    {
        private string webConfigDirectory;

        public override bool Execute()
        {
            var authTask = new AuthTaskHelper(this);
            string webConfigPath = Path.Combine(webConfigDirectory, "web.config");
            return authTask.RestoreWebConfig(webConfigPath);
        }

        /// <summary>
        /// The directory of the web server where the web.config file is found.
        /// </summary>
        [Required]
        public string WebConfigDirectory
        {
            get { return webConfigDirectory; }
            set { webConfigDirectory = value; }
        }
    }
}
