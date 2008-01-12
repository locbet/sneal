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
using Sys = System;

namespace Sneal.Build.ConfigPoke.Console
{
    internal class Program
    {
        private static int Main(string[] args)
        {
            if (args.Length < 3)
            {
                Usage();
                return 1;
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
                    using (ConfigPropertiesReader propReader = new ConfigPropertiesReader(propertyFile))
                    {
                        properties.AddPropertiesFromReader(propReader);
                    }
                }

                ConfigFile config = new ConfigFile(templateConfig);
                config.ReplacePropertyPlaceholdersWith(properties).SaveAs(destConfig);
            }
            catch (ConfigPokeException ex)
            {
                System.Console.WriteLine(ex.Message);
                return 1;
            }
            catch (Exception ex)
            {
                System.Console.WriteLine(ex.Message);
                System.Console.WriteLine();
                System.Console.WriteLine(ex.StackTrace);
                return 1;
            }

            return 0;
        }

        private static void Usage()
        {
            System.Console.WriteLine(@"ConfigPokeConsole.exe");
            System.Console.WriteLine();
            System.Console.WriteLine(
                "Utility for creating configuration files using a config template one or more property file(s).");
            System.Console.WriteLine();
            System.Console.WriteLine(
                "Note - Property files are order dependant.  Any properties appearing in subsequent files override existing properties if already present.");
            System.Console.WriteLine();
            System.Console.WriteLine("Usage:");
            System.Console.WriteLine(
                "  ConfigPokeConsole.exe <config template path> <output config path> <property file1> [property file2 [...]]");
            System.Console.WriteLine();
            System.Console.WriteLine(@"Example Usage:");
            System.Console.WriteLine(@"  ConfigPokeConsole.exe web.config.template web.config app.properties");
            System.Console.WriteLine(
                @"  ConfigPokeConsole.exe ..\..\Config\web.config.template ..\..\web.config ..\..\Config\app.properties");
            System.Console.WriteLine(
                @"  ConfigPokeConsole.exe web.config.template web.config app.properties c:\myconfigs\app.properties.john");
        }
    }
}