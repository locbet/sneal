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

using System.IO;
using System.Text;

namespace Sneal.Build.ConfigPoke
{
    public class ConfigFile : IConfigFile
    {
        public string PropertyKeyPrefix = "${";
        public string PropertyKeySuffix = "}";

        private string configContents;

        public ConfigFile(string configFilePath)
        {
            using (StreamReader reader = new StreamReader(configFilePath, Encoding.UTF8))
            {
                configContents = reader.ReadToEnd();
            }
        }

        public ConfigFile ReplacePropertyPlaceholdersWith(ConfigPropertiesCollection properties)
        {
            foreach (string propertyKey in properties.Keys)
            {
                string val = properties[propertyKey];
                string placeholder = PropertyKeyPrefix + propertyKey + PropertyKeySuffix;
                configContents = configContents.Replace(placeholder, val);
            }

            return this;
        }

        public void SaveAs(string destConfigPath)
        {
            using (StreamWriter writer = new StreamWriter(destConfigPath, false, Encoding.UTF8))
            {
                writer.Write(configContents);
            }
        }
    }
}