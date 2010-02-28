#region license
// Copyright 2010 Shawn Neal (sneal@sneal.net)
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
using System.Runtime.InteropServices;

namespace Sneal.TestUtils.Web
{
    /// <summary>
    /// The Visual Studio version.
    /// </summary>
    public enum VisualStudioVersion
    {
        /// <summary>
        /// Visual Studio 2008
        /// </summary>
        Vs2008,

        /// <summary>
        /// Visual Studio 2010
        /// </summary>
        Vs2010
    }

    /// <summary>
    /// Strategy for finding the Visual Studio WebDev.WebServer.exe that is
    /// installed with Visual Studio.
    /// </summary>
    public class VisualStudioWebDevServerFinder
    {
        private const string WebDevWebServerExe = "WebDev.WebServer.exe";
        private const string WebDevWebServer20Exe = "WebDev.WebServer20.exe";
        private const string WebDevWebServer40Exe = "WebDev.WebServer40.exe";

        /// <summary>
        /// Creates a new VS webdev.webserver finder instance.
        /// </summary>
        public VisualStudioWebDevServerFinder()
        {
            UseDotNet40Runtime = true;
            VisualStudioVersion = VisualStudioVersion.Vs2010;
        }

        /// <summary>
        /// Should the web server run under the .NET 4.0 runtime, defaults to true.
        /// </summary>
        public bool UseDotNet40Runtime { get; set; }

        /// <summary>
        /// The VS version, defaults to 2010.
        /// </summary>
        public VisualStudioVersion VisualStudioVersion { get; set; }

        /// <summary>
        /// Finds the path to the webdev.webserver.exe.
        /// </summary>
        /// <returns>The full exe path if found.</returns>
        /// <exception cref="FileNotFoundException">If the exe is not found</exception>
        public string FindWebDevWebServer()
        {
            // look in VS 2010 commons files folder
            string commonFilesDir = GetWebDevServerCommonFilesFolder();
            string exePath = CreateWebDevServerExeFullPath(commonFilesDir);

            // try the .NET runtime directory
            if (!File.Exists(exePath))
            {
                // This could point to the wrong .NET version if run from .NET 2.0 and using VS 2010
                exePath = CreateWebDevServerExeFullPath(RuntimeEnvironment.GetRuntimeDirectory());
            }

            AssertWebDevServerWasFound(exePath);
            return exePath;
        }

        private void AssertWebDevServerWasFound(string exePath)
        {
            if (!File.Exists(exePath))
            {
                throw new FileNotFoundException(
                    "Could not find the path to the Visual Studio 2010 built in web server.",
                    GetWebDevServerExeName());
            }
        }

        private string GetWebDevServerExeName()
        {
            if (VisualStudioVersion == VisualStudioVersion.Vs2010)
            {
                if (UseDotNet40Runtime)
                {
                    return WebDevWebServer40Exe;
                }
                return WebDevWebServer20Exe;
            }
            return WebDevWebServerExe;
        }

        private string CreateWebDevServerExeFullPath(string directory)
        {
            return Path.Combine(directory, GetWebDevServerExeName());
        }

        private string GetWebDevServerCommonFilesFolder()
        {
            string commonFilesPath = @"Common Files\microsoft shared\DevServer\";
            commonFilesPath += VisualStudioVersion == VisualStudioVersion.Vs2010 ? @"10.0\" : @"9.0\";
            string programFilePath = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
            return Path.Combine(programFilePath, commonFilesPath);
        }
    }
}
