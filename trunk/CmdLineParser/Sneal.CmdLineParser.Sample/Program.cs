#region license
// Copyright 2009 Shawn Neal (neal.shawn@gmail.com)
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

namespace Sneal.CmdLineParser.Sample
{
    class Program
    {
        static int Main(string[] args)
        {
            var parser = new CommandLineParser<MyOptions>(args);
            MyOptions options = parser.BuildOptions();

            if (options.ShowHelp)
            {
                DisplayHelpScreen(parser, options);
                return 0;
            }

            if (parser.IsMissingRequiredOptions())
            {
                foreach (Option missingOption in parser.FindMissingRequiredOptions())
                {
                    Console.WriteLine("Missing required option: " + missingOption.Name);
                }
                DisplayHelpScreen(parser, options);
                return 1;
            }
            Console.WriteLine(options.ToString());
            return 0;
        }

        static void DisplayHelpScreen(CommandLineParser<MyOptions> parser, MyOptions options)
        {
            Console.WriteLine("Utility for connecting to an HTTP server.");
            Console.WriteLine();
            Console.WriteLine(parser.GetUsageLines(options));
        }
    }

    public class MyOptions
    {
        [Option(ShortName = "s", LongName = "serverisreallyfunandcool",
            Description = "The remote endpoint address of the HTTP server as specified using a fully qualified domain name.",
            Required = true)]
        public string Server { get; set; }

        [Option(ShortName = "p", LongName = "port",
            Description = "The HTTP server port, which if not specified defaults to port 80.")]
        public int Port { get; set; }

        [Option(ShortName = "l", LongName = "login", Description = "Set to true to login to the server.")]
        public bool Login { get; set; }

        [Option(ShortName = "h", LongName = "help", Description = "Shows this screen.")]
        public bool ShowHelp { get; set; }

        public MyOptions()
        {
            Port = 80;
        }

        public override string ToString()
        {
            return string.Format(
                "Server={0}; Port={1}; Login={2}",
                Server, Port, Login);
        }
    }
}
