#region license
// Copyright 2009 Shawn Neal (sneal@sneal.net)
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
using Sneal.Core.IO;
using Sneal.Preconditions;

namespace Sneal.TestUtils.Web
{
    /// <summary>
    /// Wraps the .NET 2.0 webdev.webserver.exe.
    /// </summary>
    public class WebDevWebServer : IDisposable
    {
        // Dynamic port range 49152–65535
        public const int MinPort = 49152;
        public const int MaxPort = 65535;

        private Process webDevProcess;
        private FreeTcpPortFinder freePortFinder = new FreeTcpPortFinder();
        private string webRootDirectory;
        private string webDevServerPath;
        private int port;

        public WebDevWebServer(string webRootDirectory)
        {
            Throw.If(webRootDirectory, "webRootDirectory").IsNullOrEmpty();
            this.webRootDirectory = PathUtils.NormalizePath(webRootDirectory);
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
            get
            {
                if (WebServerPort == 80)
                {
                    return "http://localhost/";
                }
                return string.Format("http://localhost:{0}/", WebServerPort);
            }
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
                    port = freePortFinder.FindFreePort(80);
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

        /// <summary>
        /// Starts the web dev server running on the specified port.
        /// </summary>
        public virtual void Start()
        {
            Stop();
            VerifyWebRootDirectoryExists();
            webDevProcess = Process.Start(WebDevServerPath, CreateCommandLineArgs());
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

        private void VerifyWebRootDirectoryExists()
        {
            if (!Directory.Exists(webRootDirectory))
            {
                throw new DirectoryNotFoundException(
                    string.Format(
                        "Cannot start the web server because the web root directory {0} does not exists",
                        webRootDirectory));
            }
        }

        private string CreateCommandLineArgs()
        {
            return string.Format("/port:{0} /path:{1}", WebServerPort, webRootDirectory);
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
                    "webdev.webserver.exe");
            }
        }

        public void Dispose()
        {
            Stop();
        }
    }
}