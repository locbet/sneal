using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace Sneal.SqlMigrationConsole
{
    internal class Program
    {
        private static int Main(string[] args)
        {
            List<string> argList = new List<string>(args);

            string server = GetArgValue("-S:", argList);
            string inputFile = GetArgValue("-i:", argList);
            bool useTrustedConnection = argList.Contains("-E");

            string username;
            string password;
            if (!useTrustedConnection)
            {
                username = GetArgValue("-U:", argList);
                password = GetArgValue("-P:", argList);

                if (string.IsNullOrEmpty(username))
                {
                    Usage();
                    return 1;
                }
            }
            
            // TODO: Finish implemnting the console arg parsing
            // TODO: Move this code to it's own instance class.

            if (argList.Count > 0)
            {
                Usage();
                return 1;
            }

            return 0;
        }

        private static string GetArgValue(string argPrefix, List<string> argList)
        {
            string rawArg = argList.Find(delegate(string arg)
            {
                return (arg.StartsWith(argPrefix));
            });

            if (rawArg == null)
                return rawArg;

            argList.Remove(rawArg);

            return rawArg.Substring(argPrefix.Length);
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
                    Console.WriteLine(reader.ReadToEnd());
                }
            }
        }
    }
}