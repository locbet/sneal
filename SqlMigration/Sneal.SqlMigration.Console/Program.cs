using System;
using System.IO;
using System.Reflection;
using Sneal.CmdLineParser;

namespace Sneal.SqlMigration.Console
{
    internal class Program
    {
        public const int Success = 0;
        public const int MigrationError = -1;
        public const int UnknownError = -2;

        private static int Main(string[] args)
        {
            try
            {
                CommandLineParser parser = new CommandLineParser(args);
                parser.RegisterPropertySetter(new ListPropertySetter());

                CmdLineScriptingOptions scriptOptions = parser.BuildOptions(new CmdLineScriptingOptions());
                SourceConnectionSettings srcConnSettings = parser.BuildOptions(new SourceConnectionSettings());
                TargetConnectionSettings targetConnSettings = parser.BuildOptions(new TargetConnectionSettings());

                if (scriptOptions.ShowHelp || args.Length == 0)
                {
                    ShowUsage(scriptOptions, srcConnSettings, targetConnSettings);
                    return Success;
                }

                MigrationConsole app = new MigrationConsole();
                return app.Run(scriptOptions, srcConnSettings, targetConnSettings);
            }
            catch (SqlMigrationException ex)
            {
                System.Console.WriteLine();
                System.Console.WriteLine(ex.Message);
                System.Console.WriteLine();

                return MigrationError;                
            }
            catch (Exception ex)
            {
                System.Console.WriteLine();
                System.Console.WriteLine(ex.Message);
                System.Console.WriteLine();
                System.Console.WriteLine(ex.StackTrace);

                return UnknownError;
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

            PrintUsageLines(scriptOptions);
            PrintUsageLines(srcConnSettings);
            PrintUsageLines(targetConnSettings);
        }

        private static void PrintUsageLines(object optionsObject)
        {
            if (optionsObject == null)
                return;

            foreach (string line in CommandLineParser.GetUsageLines(optionsObject))
                System.Console.WriteLine(line);            
        }
    }
}