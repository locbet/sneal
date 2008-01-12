#region license
// Copyright 2008 Shawn Neal (neal.shawn@gmail.com)
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
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace Sneal.Build.ConfigPoke.MSBuild
{
    public class ConfigPoke : Task
    {
        private string destConfig;
        private ITaskItem[] propertyFiles;
        private string templateConfig;

        [Required]
        public string TemplateConfig
        {
            get { return templateConfig; }
            set { templateConfig = value; }
        }

        [Required]
        public string DestConfig
        {
            get { return destConfig; }
            set { destConfig = value; }
        }

        [Required]
        public ITaskItem[] PropertyFiles
        {
            get { return propertyFiles; }
            set { propertyFiles = value; }
        }

        public override bool Execute()
        {
            if (!File.Exists(templateConfig))
            {
                Log.LogError("Cannot find config template file {0}", templateConfig);
                return false;
            }

            try
            {
                ConfigPropertiesCollection properties = new ConfigPropertiesCollection();
                foreach (ITaskItem propertyFileItem in propertyFiles)
                {
                    if (propertyFileItem.ItemSpec.Length == 0)
                        continue;

                    string propertyFile = propertyFileItem.ItemSpec;

                    if (!File.Exists(propertyFile))
                    {
                        Log.LogError("Cannot find config property file {0}", propertyFile);
                        return false;
                    }

                    using (ConfigPropertiesReader propReader = new ConfigPropertiesReader(propertyFile))
                    {
                        Log.LogMessage("Adding property file {0}", propertyFileItem.ItemSpec);
                        properties.AddPropertiesFromReader(propReader);
                    }
                }

                if (!File.Exists(templateConfig))
                {
                    Log.LogError("Cannot find config template file {0}", templateConfig);
                    return false;
                }

                Log.LogMessage("Creating config file {0}", destConfig);
                ConfigFile config = new ConfigFile(templateConfig);
                config.ReplacePropertyPlaceholdersWith(properties).SaveAs(destConfig);
            }
            catch (ConfigPokeException ex)
            {
                Log.LogError(ex.Message);
            }
            catch (Exception ex)
            {
                Log.LogError(ex.Message);
            }

            return !Log.HasLoggedErrors;
        }
    }
}