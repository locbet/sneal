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
using System.Threading;

namespace Sneal.JsUnitUtils.Browsers
{
    public class FireFoxBrowser : IWebBrowser
    {
        private Process process;
        private string iexplorePath;

        private void FindFireFox()
        {
            string programFilePath = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
            iexplorePath = Path.Combine(programFilePath, @"Mozilla Firefox\firefox.exe");

            if (!File.Exists(iexplorePath))
            {
                throw new FileNotFoundException(
                    "Could not find the path to Firefox, either you don't have it installed " +
                    "or your environment is not supported or tested.",
                    "firefox.exe");
            }
        }

        public string FireFoxPath
        {
            get
            {
                if (string.IsNullOrEmpty(iexplorePath))
                    FindFireFox();

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
                FireFoxPath,
                url.ToString());
        }

        public void Close()
        {
            if (process == null || process.HasExited)
                return;

            process.CloseMainWindow();

            int count = 0;
            while (!process.HasExited && count < 10)
            {
                Thread.Sleep(1000);
                count++;
            }

            if (!process.HasExited)
                process.Kill();
        }

        public With BrowserType
        {
            get { return With.FireFox; }
        }
    }
}
