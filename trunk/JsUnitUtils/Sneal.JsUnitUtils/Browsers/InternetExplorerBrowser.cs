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

namespace Sneal.JsUnitUtils.Browsers
{
    /// <summary>
    /// Wraps an instance of IE on the local system.
    /// </summary>
    public class InternetExplorerBrowser : IWebBrowser
    {
        private Process process;
        private string iexplorePath;

        private void FindInternetExplorer()
        {
            string programFilePath = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
            iexplorePath = Path.Combine(programFilePath, @"Internet Explorer\iexplore.exe");

            if (!File.Exists(iexplorePath))
            {
                throw new FileNotFoundException(
                    "Could not find the path to Internet Explorer, either you don't have it installed " + 
                    "or your environment is not supported or tested.",
                    "iexplore.exe");
            }
        }

        public string InternetExplorerPath
        {
            get
            {
                if (string.IsNullOrEmpty(iexplorePath))
                    FindInternetExplorer();

                return iexplorePath;
            }
        }

        public void Dispose()
        {
            Close();
        }

        public void OpenUrl(Uri url)
        {
            process = Process.Start(
                InternetExplorerPath,
                url.ToString());
        }

        public void Close()
        {
            if (process == null || process.HasExited)
                return;

            process.CloseMainWindow();
        }

        public With BrowserType
        {
            get { return With.InternetExplorer; }
        }
    }
}
