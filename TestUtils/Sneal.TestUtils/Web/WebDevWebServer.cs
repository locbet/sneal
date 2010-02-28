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
using Sneal.Core;
using Sneal.Core.IO;

namespace Sneal.TestUtils.Web
{
    /// <summary>
    /// Wraps the .NET 2.0 webdev.webserver.exe.
    /// </summary>
    public class WebDevWebServer : IDisposable
    {
        // Default HTTP port
        private const int DefaultPort = 80;

        private readonly FreeTcpPortFinder _freePortFinder = new FreeTcpPortFinder();

        private Process _webDevProcess;
        private readonly string _webRootDirectory;
        private int _port;

        /// <summary>
        /// Creates a new wrapper around the webdev.webserver.exe.
        /// </summary>
        /// <param name="webRootDirectory">The local web root path</param>
        /// <param name="webDevServerPath">The full path to the webdev.webserver.exe</param>
        public WebDevWebServer(string webRootDirectory, string webDevServerPath)
        {
            Guard.AgainstNullOrEmpty(() => webRootDirectory);
            Guard.AgainstNullOrEmpty(() => webDevServerPath);

            var pathBuilder = new PathBuilder();
            _webRootDirectory = pathBuilder.Normalize(webRootDirectory);
            WebDevServerPath = pathBuilder.Normalize(webDevServerPath);
        }

        /// <summary>
        /// Creates a new webdev.webserver wrapper for use with VS 2010 and the
        /// .NET 4.0 runtime.
        /// </summary>
        /// <param name="webRootDirectory">The local web root path</param>
        /// <returns>An unstarted web server instance</returns>
        public static WebDevWebServer Vs2010WithDotNet40(string webRootDirectory)
        {
            var finder = new VisualStudioWebDevServerFinder
            {
                UseDotNet40Runtime = true,
                VisualStudioVersion = VisualStudioVersion.Vs2010
            };
            return new WebDevWebServer(webRootDirectory, finder.FindWebDevWebServer());
        }

        /// <summary>
        /// Creates a new webdev.webserver wrapper for use with VS 2010 and the
        /// .NET 2.0 runtime (includes .NET 3.0 and 3.5).
        /// </summary>
        /// <param name="webRootDirectory">The local web root path</param>
        /// <returns>An unstarted web server instance</returns>
        public static WebDevWebServer Vs2010WithDotNet20(string webRootDirectory)
        {
            var finder = new VisualStudioWebDevServerFinder
            {
                UseDotNet40Runtime = false,
                VisualStudioVersion = VisualStudioVersion.Vs2010
            };
            return new WebDevWebServer(webRootDirectory, finder.FindWebDevWebServer());
        }

        /// <summary>
        /// Creates a new webdev.webserver wrapper for use with VS 2008 and the
        /// .NET 2.0 runtime (includes .NET 3.0 and 3.5).
        /// </summary>
        /// <param name="webRootDirectory">The local web root path</param>
        /// <returns>An unstarted web server instance</returns>
        public static WebDevWebServer Vs2008(string webRootDirectory)
        {
            var finder = new VisualStudioWebDevServerFinder
            {
                UseDotNet40Runtime = false,
                VisualStudioVersion = VisualStudioVersion.Vs2008
            };
            return new WebDevWebServer(webRootDirectory, finder.FindWebDevWebServer());
        }

        /// <summary>
        /// The full path to the webdev.wevserver.exe.  This makes an
        /// attempt to find the exe at standard locations.
        /// </summary>
        public virtual string WebDevServerPath { get; private set; }

        /// <summary>
        /// The root web site bin directory where the site's DLLs are located.
        /// Something like c:\source\mytests\bin
        /// </summary>
        public virtual string WebBinDirectory
        {
            get { return Path.Combine(_webRootDirectory, "bin"); }
        }

        /// <summary>
        /// Gets the web root directory as an http address.
        /// </summary>
        /// <remarks>Something like http://localhost:8080/</remarks>
        public virtual string WebRootHttpPath
        {
            get
            {
                if (WebServerPort == DefaultPort)
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
                if (_port == 0)
                {
                    _port = _freePortFinder.FindFreePort(80);
                }

                return _port;
            }
        }

        /// <summary>
        /// The root website directory hosting the web pages.  Something
        /// like c:\tools\jsunit
        /// </summary>
        public string WebRootDirectory
        {
            get { return _webRootDirectory; }
        }

        /// <summary>
        /// Starts the web dev server running on the specified port.
        /// </summary>
        public virtual void Start()
        {
            Stop();
            AssertWebRootDirectoryExists();
            _webDevProcess = Process.Start(WebDevServerPath, CreateCommandLineArgs());
        }

        /// <summary>
        /// Stops the web server if its currently running.
        /// </summary>
        public virtual void Stop()
        {
            if (_webDevProcess != null)
            {
                _webDevProcess.Kill();
            }
        }

        public void Dispose()
        {
            Stop();
        }

        private void AssertWebRootDirectoryExists()
        {
            if (!Directory.Exists(_webRootDirectory))
            {
                throw new DirectoryNotFoundException(
                    string.Format(
                        "Cannot start the web server because the web root directory {0} does not exist",
                        _webRootDirectory));
            }
        }

        private string CreateCommandLineArgs()
        {
            return string.Format("/port:{0} /path:{1}", WebServerPort, _webRootDirectory);
        }

    }
}