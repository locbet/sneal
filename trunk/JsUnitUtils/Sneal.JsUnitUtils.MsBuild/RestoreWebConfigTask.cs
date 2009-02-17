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

using System.IO;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace Sneal.JsUnitUtils.MsBuild
{
    /// <summary>
    /// Restores a web.config.bak to web.config in the specified directory.
    /// </summary>
    public class RestoreWebConfigTask : Task
    {
        private string webConfigDirectory;

        public override bool Execute()
        {
            var authTask = new AuthTaskHelper(this);
            string webConfigPath = Path.Combine(webConfigDirectory, "Web.config");
            return authTask.RestoreWebConfig(webConfigPath);
        }

        /// <summary>
        /// The directory of the web server where the web.config file is found.
        /// </summary>
        [Required]
        public string WebConfigDirectory
        {
            get { return webConfigDirectory; }
            set { webConfigDirectory = value; }
        }
    }
}
