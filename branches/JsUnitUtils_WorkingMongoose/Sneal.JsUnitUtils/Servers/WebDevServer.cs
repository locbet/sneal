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
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using Sneal.JsUnitUtils.Utils;
using Sneal.Preconditions.Aop;

namespace Sneal.JsUnitUtils.Servers
{
    /// <summary>
    /// Wraps the .NET 2.0 webdev.webserver.exe.
    /// </summary>
    public class WebDevServer : IWebServer
    {
        private const string WebDevExe = "WebDev.WebServer.exe";

        private readonly IDiskProvider diskProvider;
        private Process webDevProcess;
        private string webRootDirectory;
        private string webDevServerPath;
        private int port;

        public WebDevServer(
            [NotNull] IDiskProvider diskProvider,
            [NotNullOrEmpty] string webRootDirectory,
            int port)
        {
            this.webRootDirectory = diskProvider.NormalizePath(webRootDirectory);
            this.diskProvider = diskProvider;
            this.port = port;
        }

        protected IDiskProvider DiskProvider
        {
            get { return diskProvider; }
        }

        /// <summary>
        /// The full path to the webdev.wevserver.exe.  This makes an
        /// attempt to find the exe at standard locations.
        /// </summary>
        public virtual string WebDevServerPath
        {
            get
            {
                if (string.IsNullOrEmpty(webDevServerPath))
                    FindWebDevServer();

                return webDevServerPath;
            }
            set { webDevServerPath = value; }
        }

        public virtual string WebBinDirectory
        {
            get { return Path.Combine(webRootDirectory, "bin"); }
        }

        public virtual string WebRootHttpPath
        {
            get { return string.Format("http://localhost:{0}/", WebServerPort); }
        }

        public virtual int WebServerPort
        {
            get { return port; }
        }

        public string WebRootDirectory
        {
            get { return webRootDirectory; }
            set { webRootDirectory = value; }
        }

        public string MakeHttpUrl(string localFilePath)
        {
            string normalizedPath = diskProvider.NormalizePath(localFilePath);
            if (!Path.IsPathRooted(normalizedPath))
            {
                return diskProvider.Combine(WebRootHttpPath, normalizedPath);
            }

            string relativePath = diskProvider.MakePathRelative(WebRootDirectory, normalizedPath);
            return diskProvider.Combine(WebRootHttpPath, relativePath);
        }

        /// <summary>
        /// Starts the web dev server running on the specified port.
        /// </summary>
        public virtual DisposableAction Start()
        {
            Stop();

            string webDevArgs = string.Format("/port:{0} /path:{1}", WebServerPort, webRootDirectory);
            webDevProcess = Process.Start(WebDevServerPath, webDevArgs);

            return new DisposableAction(Stop);
        }

        /// <summary>
        /// Stops the web server if its currently running.
        /// </summary>
        public virtual void Stop()
        {
            if (webDevProcess != null)
            {
                webDevProcess.Kill();
            }
        }

        protected virtual void FindWebDevServer()
        {
            // look in .NET 2.0 framework folder first
            webDevServerPath = Path.Combine(RuntimeEnvironment.GetRuntimeDirectory(), WebDevExe);
            if (!File.Exists(webDevServerPath))
            {
                // look in VS 2008 commons files folder
                string programFilePath = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
                webDevServerPath = Path.Combine(
                    programFilePath,
                    @"Common Files\microsoft shared\DevServer\9.0\" + WebDevExe);
            }

            if (!File.Exists(webDevServerPath))
            {
                throw new FileNotFoundException(
                    "Could not find the path to the .NET 2.0 built in web server.",
                    WebDevExe);
            }
        }
    }
}