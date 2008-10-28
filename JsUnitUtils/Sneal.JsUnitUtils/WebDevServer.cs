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
using System.Net.Sockets;
using System.Runtime.InteropServices;
using Sneal.JsUnitUtils.Utils;
using Sneal.Preconditions.Aop;

namespace Sneal.JsUnitUtils
{
    /// <summary>
    /// Wraps the .NET 2.0 webdev.webserver.exe.
    /// </summary>
    public class WebDevServer : IWebServer
    {
        // Dynamic port range 49152–65535
        public const int MinPort = 49152;
        public const int MaxPort = 65535;

        private readonly IDiskProvider diskProvider;
        private Process webDevProcess;
        private string webRootDirectory;
        private string webDevServerPath;
        private int port;

        public WebDevServer([NotNull] IDiskProvider diskProvider)
        {
            this.diskProvider = diskProvider;
        }

        public WebDevServer([NotNull] IDiskProvider diskProvider, [NotNullOrEmpty] string webRootDirectory)
            : this(diskProvider)
        {
            this.webRootDirectory = diskProvider.NormalizePath(webRootDirectory);
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

        /// <summary>
        /// The root web site bin directory where the site's DLLs are located.
        /// Something like c:\source\mytests\bin
        /// </summary>
        public virtual string WebBinDirectory
        {
            get { return Path.Combine(webRootDirectory, "bin"); }
        }

        /// <summary>
        /// Gets the web root directory as an http address.
        /// </summary>
        /// <remarks>Something like http://localhost:8080/</remarks>
        public virtual string WebRootHttpPath
        {
            get { return string.Format("http://localhost:{0}/", WebServerPort); }
        }

        /// <summary>
        /// The port this web server is running on, which is commonly port 80.
        /// </summary>
        public virtual int WebServerPort
        {
            get
            {
                if (port == 0)
                {
                    FindFreePort();
                }

                return port;
            }
        }

        /// <summary>
        /// The root website directory hosting the web pages.  Something
        /// like c:\tools\jsunit
        /// </summary>
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

        protected virtual void FindFreePort()
        {
            int portCandidate;
            do
            {
                portCandidate = GetRandomDynamicPort();
            } while (!IsPortOpen(portCandidate));

            port = portCandidate;
        }

        private static bool IsPortOpen(int port)
        {
            var tcpClient = new TcpClient();
            try
            {
                tcpClient.Connect("127.0.0.1", port);
                tcpClient.Close();
                return false;
            }
            catch (SocketException)
            {
            }

            return true;
        }

        private static int GetRandomDynamicPort()
        {
            Random ran = new Random(DateTime.Now.Millisecond);
            return ran.Next(MinPort, MaxPort);              
        }

        protected virtual void FindWebDevServer()
        {
            // look in .NET 2.0 framework folder first
            webDevServerPath = Path.Combine(RuntimeEnvironment.GetRuntimeDirectory(), "WebDev.WebServer.exe");
            if (!File.Exists(webDevServerPath))
            {
                // look in VS 2008 commons files folder
                string programFilePath = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
                webDevServerPath = Path.Combine(
                    programFilePath,
                    @"Common Files\microsoft shared\DevServer\9.0\WebDev.WebServer.exe");
            }

            if (!File.Exists(webDevServerPath))
            {
                throw new FileNotFoundException(
                    "Could not find the path to the .NET 2.0 built in web server.",
                    "webdev.server.exe");
            }
        }
    }
}