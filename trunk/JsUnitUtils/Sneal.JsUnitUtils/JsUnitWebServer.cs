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

namespace Sneal.JsUnitUtils
{
    /// <summary>
    /// Uses the webdev.webserver to start an ASHX handler that accepts
    /// posted JSUnit results.
    /// </summary>
    public class JsUnitWebServer : WebDevServer
    {
        private readonly ITemplates templates;

        public JsUnitWebServer(ITemplates templates)
        {
            this.templates = templates;
            SetWebRootDirectory();
        }

        private void SetWebRootDirectory()
        {
            string localAppDataDir = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            WebRootDirectory = Path.Combine(localAppDataDir, "JsUnitUtils");            
        }

        /// <summary>
        /// Gets the path to the result handler.  Something like
        /// http://localhost:5100/JsUnitResultHandler.ashx
        /// </summary>
        public string HandlerAddress
        {
            get
            {
                return string.Format("http://localhost:{0}/{1}", WebServerPort, templates.AshxHandlerFileName);
            }
        }

        /// <summary>
        /// Starts the web dev server running on the specified port.
        /// </summary>
        public override DisposableAction Start()
        {
            CreateWebDirectoryContent();
            return base.Start();
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
            string webAssemblyFileName = Assembly.GetExecutingAssembly().GetName().Name + ".dll";
            string webAssemblySrcPath = Path.Combine(WebBinDirectory, webAssemblyFileName);
            File.Copy(webAssemblyFileName, webAssemblySrcPath, true);
        }

        private void CreateWebServerDirectories()
        {
            System.IO.Directory.CreateDirectory(WebRootDirectory);
            System.IO.Directory.CreateDirectory(WebBinDirectory);
        }
    }
}