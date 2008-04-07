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

            MigrationConsole app = new MigrationConsole();
            return app.Run(options);
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