using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Sneal.CmdLineParser;

namespace Sneal.SqlMigration.Console
{
    internal class Program
    {
        private static int Main(string[] args)
        {
            CommandLineParser parser = new CommandLineParser(args);
            Options options = parser.BuildOptions(new Options());

            return 1;
        }

        private static void Usage()
        {
            using (
                Stream s =
                    Assembly.GetExecutingAssembly().GetManifestResourceStream(
                        "Sneal.SqlMigrationConsole.Usage.txt"))
            {
                using (StreamReader reader = new StreamReader(s))
                {
                    System.Console.WriteLine(reader.ReadToEnd());
                }
            }
        }
    }
}