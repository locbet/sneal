using System.IO;
using System.Xml;
using Microsoft.Build.Utilities;
using Sneal.JsUnitUtils.Utils;

namespace Sneal.JsUnitUtils.MsBuild
{
    public class AuthTaskHelper
    {
        private readonly Task task;

        public AuthTaskHelper(Task task)
        {
            this.task = task;
        }

        /// <summary>
        /// Makes a backup copy of the web.config and then removes the
        /// authorization node.
        /// </summary>
        /// <param name="webConfigPath">The full path to the web.config</param>
        /// <returns><c>true</c> if successful</returns>
        public virtual bool DisableWebConfigAuthorization(string webConfigPath)
        {
            if (!File.Exists(webConfigPath))
            {
                Log.LogError("Cannot find the web.config in the root web directory: {0}", webConfigPath);
                return false;
            }

            if (!BackupWebConfig(webConfigPath))
            {
                return false;
            }

            using (FileAsWritable(webConfigPath))
            {
                RemoveAuthorizationNodeFromWebConfig(webConfigPath);
            }

            return true;
        }

        /// <summary>
        /// Makes a backup copy of the web.config named web.config.bak in the
        /// same directory.
        /// </summary>
        /// <param name="webConfigPath">The full path to the web.config</param>
        public bool BackupWebConfig(string webConfigPath)
        {
            if (!File.Exists(webConfigPath))
            {
                Log.LogError("Cannot find {0}", webConfigPath);
                return false;
            }

            string backupWebConfigPath = webConfigPath + ".bak";
            using (FileAsWritable(backupWebConfigPath))
            {
                File.Copy(webConfigPath, backupWebConfigPath, true);
            }

            return true;
        }

        /// <summary>
        /// Makes a backup copy of the web.config named web.config.bak in the
        /// same directory.
        /// </summary>
        /// <param name="webConfigPath">The full path to the web.config</param>
        public bool RestoreWebConfig(string webConfigPath)
        {
            string backupWebConfigPath = webConfigPath + ".bak";

            if (!File.Exists(backupWebConfigPath))
            {
                Log.LogError("Cannot find the web.config backup {0}", backupWebConfigPath);
                return false;
            }

            using (FileAsWritable(webConfigPath))
            {
                File.Copy(backupWebConfigPath, webConfigPath, true);
            }

            return true;
        }

        private static void RemoveAuthorizationNodeFromWebConfig(string path)
        {
            var webConfig = new XmlDocument();
            webConfig.Load(path);

            var authNode = webConfig.SelectSingleNode("//authorization");
            if (authNode == null)
            {
                return;
            }

            webConfig.RemoveChild(authNode);
            webConfig.Save(path);
        }

        /// <summary>
        /// If the file is writable, nothing is done.  If the file is readonly
        /// then the file is set to writable during use, then set back to
        /// readonly when the operation is complete.
        /// </summary>
        /// <param name="file">The file to ensure writability of</param>
        /// <returns>The cleanup delegate</returns>
        private static DisposableAction FileAsWritable(string file)
        {
            if (!IsFileReadOnly(file))
            {
                return new DisposableAction(/* no-op */);
            }

            FileAttributes originalFileAttributes = File.GetAttributes(file);
            File.SetAttributes(file, FileAttributes.Normal);
            return new DisposableAction(() => File.SetAttributes(file, originalFileAttributes));
        }

        private static bool IsFileReadOnly(string path)
        {
            if (!File.Exists(path))
            {
                return false;
            }

            return (File.GetAttributes(path) & FileAttributes.ReadOnly) == 0;
        }

        protected virtual TaskLoggingHelper Log
        {
            get { return task.Log; }
        }
    }
}
