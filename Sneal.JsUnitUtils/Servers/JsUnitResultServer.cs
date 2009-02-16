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

using System;
using System.IO;
using System.Reflection;
using Sneal.JsUnitUtils.Utils;
using Sneal.Preconditions.Aop;

namespace Sneal.JsUnitUtils.Servers
{
    /// <summary>
    /// Uses the webdev.webserver to start an ASHX handler that accepts
    /// posted JSUnit results.
    /// </summary>
    public class JsUnitResultServer
    {
        private const int DefaultPort = 60000;

        private readonly ITemplates templates;
        private readonly IDiskProvider diskProvider;
        private readonly int port;
        private WebDevServer webDevSever;

        public JsUnitResultServer(
            [NotNull] IDiskProvider diskProvider,
            [NotNull] ITemplates templates,
            [NotNull] IFreeTcpPortFinder freePortFinder)
        {
            this.diskProvider = diskProvider;
            this.templates = templates;
            port = freePortFinder.FindFreePort(DefaultPort);
        }

        /// <summary>
        /// Run the JSUnit server from the user's local application data folder.
        /// </summary>
        /// <returns>The root web directory path</returns>
        private static string WebRootDirectory
        {
            get
            {
                string localPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                return Path.Combine(localPath, "JsUnitResultServer");
            }
        }

        /// <summary>
        /// Gets the path to the result handler.  Something like
        /// http://localhost:62031/JsUnitResultHandler.ashx
        /// </summary>
        public string HandlerAddress
        {
            get
            {
                return string.Format("http://localhost:{0}/{1}", port, templates.AshxHandlerFileName);
            }
        }

        /// <summary>
        /// Starts the web dev server running on the specified port.
        /// </summary>
        public DisposableAction Start()
        {
            webDevSever = new WebDevServer(diskProvider, WebRootDirectory, port);
            CreateWebDirectoryContent();
            return webDevSever.Start();
        }

        public void Stop()
        {
            webDevSever.Stop();
        }

        private void CreateWebDirectoryContent()
        {
            CreateWebServerDirectories();
            CopyAssemblyToWebBinDirectory();
            CreateAshxHandler();
        }

        private void CreateAshxHandler()
        {
            string ashxPath = Path.Combine(WebRootDirectory, templates.AshxHandlerFileName);
            using (var writer = new StreamWriter(ashxPath))
            {
                writer.Write(templates.GetAshxHandlerContent());
            }
        }

        private void CopyAssemblyToWebBinDirectory()
        {
            string webAssemblySrcPath = Assembly.GetExecutingAssembly().Location;
            if (!string.IsNullOrEmpty(webAssemblySrcPath))
            {
                string webAssemblyDestPath = Path.Combine(
                    webDevSever.WebBinDirectory, Path.GetFileName(webAssemblySrcPath));
                // TODO: turn this into a diskprovider call
                File.Copy(webAssemblySrcPath, webAssemblyDestPath, true);
            }
        }

        private void CreateWebServerDirectories()
        {
            diskProvider.CreateDirectory(webDevSever.WebBinDirectory);
        }
    }
}