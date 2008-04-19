using System;
using System.IO;
using System.Reflection;
using log4net;
using Sneal.CmdLineParser;

namespace Sneal.SqlMigration.Console
{
    internal class Program
    {
        public const int Success = 0;
        public const int MigrationError = -1;
        public const int UnknownError = -2;

        private static readonly ILog Log = LogManager.GetLogger(typeof (Program));

        private static int Main(string[] args)
        {
            try
            {
                CommandLineParser parser = new CommandLineParser(args);
                parser.RegisterPropertySetter(new ListPropertySetter());

                CmdLineScriptingOptions scriptOptions = parser.BuildOptions(new CmdLineScriptingOptions());
                SourceConnectionSettings srcConnSettings = parser.BuildOptions(new SourceConnectionSettings());
                TargetConnectionSettings targetConnSettings = parser.BuildOptions(new TargetConnectionSettings());

                Log4NetConfigurator.ConfigureLog4Net(scriptOptions.Log4netConfigPath);
#if DEBUG
                Log4NetConfigurator.SetLevel(Log, "DEBUG");
#endif

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
                Log.Error(ex.Message);
                Log.Debug(ex);

                return MigrationError;                
            }
            catch (Exception ex)
            {
                Log.Error(ex);

                return UnknownError;
            }
        }

        private static void ShowUsage(
            CmdLineScriptingOptions scriptOptions,
            SourceConnectionSettings srcConnSettings,
            TargetConnectionSettings targetConnSettings)
        {
            using (Stream s = Assembly.GetExecutingAssembly().GetManifestResourceStream(
                        "Sneal.SqlMigration.Console.Usage.txt"))
            {
                using (StreamReader reader = new StreamReader(s))
                {
                    Log.Info(reader.ReadToEnd());
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
                Log.Info(line);       
        }
    }
}