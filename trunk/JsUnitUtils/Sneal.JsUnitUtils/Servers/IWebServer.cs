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

using System.Collections.Specialized;
using Sneal.JsUnitUtils.Utils;
using Sneal.Preconditions.Aop;

namespace Sneal.JsUnitUtils.Servers
{
    /// <summary>
    /// Wraps an implmentation of a simple HTTP server.
    /// </summary>
    public interface IWebServer
    {
        /// <summary>
        /// The root web site bin directory where the site's DLLs are located.
        /// Something like c:\source\mytests\bin
        /// </summary>
        string WebBinDirectory { get; }

        /// <summary>
        /// Gets the web root directory as an http address.
        /// </summary>
        /// <remarks>Something like http://localhost:8080/</remarks>
        string WebRootHttpPath { get; }

        /// <summary>
        /// The port this web server is running on, which is commonly port 80.
        /// </summary>
        int WebServerPort { get; }

        /// <summary>
        /// The root website directory hosting the web pages.  Something
        /// like c:\tools\jsunit
        /// </summary>
        string WebRootDirectory { get; }

        /// <summary>
        /// Turns a local file path into an HTTP URI.  The file must either be a
        /// relative path or a qualified path underneath the web server's
        /// web root directory.
        /// </summary>
        /// <param name="localFilePath">The local file path</param>
        /// <returns>The http url of the file served from this web server.</returns>
        string MakeHttpUrl([NotNullOrEmpty] string localFilePath);

        /// <summary>
        /// Starts the web dev server running on the specified port.  Disposing
        /// of the return object Stops this instance of the web server.
        /// </summary>
        DisposableAction Start();

        /// <summary>
        /// Stops the web server if its currently running.
        /// </summary>
        void Stop();

        /// <summary>
        /// This event fires anytime data is posted to the web server.
        /// </summary>
        event DataPosted DataPostedEvent;
    }

    /// <summary>
    /// Delegate used when data is posted to the web server.
    /// </summary>
    /// <param name="formData">The key value pairs of the form data</param>
    public delegate void DataPosted(NameValueCollection formData);
}