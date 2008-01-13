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
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace Sneal.Build.ConfigPoke.Console
{
    internal class Program
    {
        private const int Success = 0;
        private const int Failure = 1;

        private static int Main(string[] args)
        {
            if (args.Length < 3)
            {
                Usage();
                return Failure;
            }

            List<string> propertyFiles = new List<string>();
            for (int idx = 2; idx < args.Length; idx++)
            {
                propertyFiles.Add(args[idx]);
            }

            return ProcessConfigs(args[0], args[1], propertyFiles);
        }

        private static int ProcessConfigs(string templateConfig, string destConfig, IEnumerable<string> propertyFiles)
        {
            try
            {
                ConfigPropertiesCollection properties = new ConfigPropertiesCollection();
                foreach (string propertyFile in propertyFiles)
                {
                    if (!File.Exists(propertyFile))
                    {
                        WriteError("Cannot find the property file {0}", propertyFile);
                        return Failure;
                    }

                    using (ConfigPropertiesReader propReader = new ConfigPropertiesReader(propertyFile))
                    {
                        properties.AddPropertiesFromReader(propReader);
                    }
                }

                if (!File.Exists(templateConfig))
                {
                    WriteError("Cannot find the config template file {0}", templateConfig);
                    return Failure;
                }

                ConfigFile config = new ConfigFile(templateConfig);
                config.ReplacePropertyPlaceholdersWith(properties).SaveAs(destConfig);
            }
            catch (ConfigPokeException ex)
            {
                WriteError(ex.Message);
                return Failure;
            }
            catch (Exception ex)
            {
                WriteError(ex.Message);
                WriteError(ex.StackTrace);
                return Failure;
            }

            return Success;
        }

        private static void Usage()
        {
            using (Stream s = Assembly.GetExecutingAssembly().GetManifestResourceStream(
                        "Sneal.Build.ConfigPoke.Console.Usage.txt"))
            {
                using (StreamReader reader = new StreamReader(s))
                {
                    System.Console.WriteLine(reader.ReadToEnd());
                }
            }
        }

        private static void WriteError(string formatMsg, params string[] parms)
        {
            System.Console.WriteLine("ERROR: " + formatMsg, parms);
        }
    }
}