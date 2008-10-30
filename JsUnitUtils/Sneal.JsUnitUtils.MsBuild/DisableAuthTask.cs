using System.IO;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace Sneal.JsUnitUtils.MsBuild
{
    /// <summary>
    /// Utility task used to disable authentication for a particular website.
    /// </summary>
    /// <remarks>
    /// Preferably you should use the web.config and add location exclusions
    /// to disable security to your javascript tests files, but that's not always
    /// workable if the files under test are all over the place.
    /// </remarks>
    public class DisableAuthTask : Task
    {
        private string webConfigDirectory;

        public override bool Execute()
        {
            var authTask = new AuthTaskHelper(this);
            string webConfigPath = Path.Combine(webConfigDirectory, "web.config");
            return authTask.DisableWebConfigAuthorization(webConfigPath);
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
