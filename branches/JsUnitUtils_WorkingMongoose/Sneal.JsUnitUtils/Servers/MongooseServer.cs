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
using System.Globalization;
using System.Threading;
using Sneal.JsUnitUtils.Utils;
using Sneal.Preconditions.Aop;

namespace Sneal.JsUnitUtils.Servers
{
    /// <summary>
    /// Provides a light weight HTTP server using Mongoose.
    /// <seealso cref="http://code.google.com/p/mongoose">Mongoose</seealso>
    /// </summary>
    public class MongooseServer : IWebServer
    {
        private readonly IDiskProvider diskProvider;
        private readonly string webRootDirectory;
        private readonly int port;
        private IntPtr context = IntPtr.Zero;
        private Thread mongooseThread;

        public MongooseServer(
            [NotNull] IDiskProvider diskProvider,
            [NotNullOrEmpty] string webRootDirectory,
            int port)
        {
            this.diskProvider = diskProvider;
            this.port = port;
            this.webRootDirectory = diskProvider.NormalizePath(webRootDirectory);
        }

        public MongooseServer(
            [NotNull] IDiskProvider diskProvider,
            [NotNullOrEmpty] string webRootDirectory)
        {
            this.diskProvider = diskProvider;
            port = new FreeTcpPortFinder().FindFreePort();
            this.webRootDirectory = diskProvider.NormalizePath(webRootDirectory);
        }

        public string WebBinDirectory
        {
            get { return diskProvider.Combine(webRootDirectory, "bin"); }
        }

        public string WebRootHttpPath
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

        public DisposableAction Start()
        {
            Stop();
            mongooseThread = new Thread(start =>
            {
                context = MongooseNative.mg_start();
                MongooseNative.mg_set_option(context, "ports", port.ToString(CultureInfo.InvariantCulture));
                MongooseNative.mg_set_option(context, "root", webRootDirectory);
            });
            mongooseThread.Start();
            return new DisposableAction(Stop);
        }

        public void Stop()
        {
            if (mongooseThread != null && mongooseThread.IsAlive)
            {
                mongooseThread.Join(60000);
            }
        }
    }
}
