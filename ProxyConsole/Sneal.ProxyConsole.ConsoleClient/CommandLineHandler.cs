using System;
using Sneal.CmdLineParser;
using Sneal.ProxyConsole.ConsoleClient.ConsoleRunnerService;

namespace Sneal.ProxyConsole.ConsoleClient
{
    public class CommandLineHandler
    {
        private readonly string[] _args;

        public CommandLineHandler(string[] args)
        {
            _args = args;
        }

        public StartExecutionMessage CreateStartExecutionMessageFromCommandLine()
        {
            CommandSwitches switches = GetSwitches();
            return new StartExecutionMessage
            {
                FileName = switches.ExecutableFileName,
                Arguments = switches.Arguments,
                Timeout = switches.Timeout,
                WorkingDirectory = switches.WorkingDirectory
            };
        }

        public CommandSwitches GetSwitches()
        {
            var cmdLineParser = new CommandLineParser(_args);
            var switches = new CommandSwitches();
            try
            {
                switches = cmdLineParser.BuildOptions(switches);
            }
            catch (CmdLineParserException ex)
            {
                Console.WriteLine(ex.Message);
                Usage();
                Environment.Exit(1);
            }
            return switches;
        }

        private static void Usage()
        {
            Console.WriteLine();
            foreach (string usageLine in CommandLineParser.GetUsageLines(new CommandSwitches()))
            {
                Console.WriteLine(usageLine);
            }
            Console.WriteLine();
            Console.WriteLine("\tSneal.ProxyConsole.ConsoleClient /ConsoleAddress=http://remotehost:8731/ConsoleRunner /Executable=ping.exe /Arguments=localhost");
            Console.WriteLine();
        }
    }
}
