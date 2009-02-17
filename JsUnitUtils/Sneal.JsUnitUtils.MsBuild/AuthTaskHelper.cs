#region license
// Copyright 2008 Shawn Neal (sneal@sneal.net)
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion

using System.IO;
using System.Xml;
using Microsoft.Build.Utilities;
using Sneal.JsUnitUtils.Utils;

namespace Sneal.JsUnitUtils.MsBuild
{
    public class AuthTaskHelper
    {
        private const string AppConfigNS = "http://schemas.microsoft.com/.NetConfiguration/v2.0";
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

            var appConfigNsMgr = new XmlNamespaceManager(webConfig.NameTable);
            appConfigNsMgr.AddNamespace("b", AppConfigNS);

            var systemwebNode = webConfig.SelectSingleNode("//b:system.web", appConfigNsMgr);
            var authNode = webConfig.SelectSingleNode("//b:authorization", appConfigNsMgr);
            if (authNode == null || systemwebNode == null)
            {
                return;
            }

            systemwebNode.RemoveChild(authNode);
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

            return (File.GetAttributes(path) & FileAttributes.ReadOnly) == FileAttributes.ReadOnly;
        }

        protected virtual TaskLoggingHelper Log
        {
            get { return task.Log; }
        }
    }
}
