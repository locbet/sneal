using System;
using System.IO;
using System.Reflection;
using Sneal.CmdLineParser;

namespace Sneal.SqlMigration.Console
{
    internal class Program
    {
        private static int Main(string[] args)
        {
            try
            {
                CommandLineParser parser = new CommandLineParser(args);
                parser.RegisterPropertySetter(new ListPropertySetter());

                CmdLineScriptingOptions scriptOptions = parser.BuildOptions(new CmdLineScriptingOptions());
                SourceConnectionSettings srcConnSettings = parser.BuildOptions(new SourceConnectionSettings());
                TargetConnectionSettings targetConnSettings = parser.BuildOptions(new TargetConnectionSettings());

                if (scriptOptions.ShowHelp)
                {
                    ShowUsage(scriptOptions, srcConnSettings, targetConnSettings);
                    return 1;
                }

                MigrationConsole app = new MigrationConsole();
                return app.Run(scriptOptions, srcConnSettings, targetConnSettings);
            }
            catch (Exception ex)
            {
                System.Console.WriteLine(ex.Message);
                System.Console.WriteLine();
                //System.Console.WriteLine(ex.StackTrace);

                return -1;
            }
        }

        private static void ShowUsage(CmdLineScriptingOptions scriptOptions,
            SourceConnectionSettings srcConnSettings, TargetConnectionSettings targetConnSettings)
        {
            using (Stream s = Assembly.GetExecutingAssembly().GetManifestResourceStream(
                        "Sneal.SqlMigration.Console.Usage.txt"))
            {
                using (StreamReader reader = new StreamReader(s))
                {
                    System.Console.WriteLine(reader.ReadToEnd());
                }
            }

            foreach (string line in CommandLineParser.GetUsageLines(scriptOptions))
                System.Console.WriteLine(line);

            foreach (string line in CommandLineParser.GetUsageLines(srcConnSettings))
                System.Console.WriteLine(line);

            foreach (string line in CommandLineParser.GetUsageLines(targetConnSettings))
                System.Console.WriteLine(line);
        }
    }
}