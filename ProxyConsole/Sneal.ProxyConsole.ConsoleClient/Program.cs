using System;
using System.ServiceModel;
using System.Threading;
using Sneal.ProxyConsole.ConsoleClient.ConsoleRunnerService;

namespace Sneal.ProxyConsole.ConsoleClient
{
    class Program
    {
        static void Main(string[] args)
        {
            var cmdLineHandler = new CommandLineHandler(args);
            CommandSwitches switches = cmdLineHandler.GetSwitches();
            StartExecutionMessage startMsg = cmdLineHandler.CreateStartExecutionMessageFromCommandLine();

            EndpointAddress endpointAddress = new EndpointAddress(switches.EndpointAddress);
            WSDualHttpBinding serviceBinding = new WSDualHttpBinding();
            if (startMsg.Timeout > 0)
                serviceBinding.ReceiveTimeout = TimeSpan.FromSeconds(startMsg.Timeout);

            var callbackHandler = new ConsoleCallbackHandler();
            var svcProxy = new ConsoleRunnerClient(new InstanceContext(callbackHandler), serviceBinding, endpointAddress);

            svcProxy.Run(startMsg);

            while (!callbackHandler.IsComplete)
            {
                Thread.Sleep(100);
            }
        }
    }
}
