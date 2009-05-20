using System;
using Sneal.ProxyConsole.ConsoleClient.ConsoleRunnerService;

namespace Sneal.ProxyConsole.ConsoleClient
{
    public class ConsoleCallbackHandler : ConsoleRunnerCallback
    {
        public bool IsComplete;
        public int ExitCode;

        public void ExecutionComplete(ExecutionFinishedMessage completeMessage)
        {
            ExitCode = completeMessage.ExitCode;
            if (completeMessage.IsError)
            {
                Console.WriteLine(completeMessage.Output);
            }
            IsComplete = true;
        }

        public void ExecutionProgress(ExecutionProgressMessage progressMessage)
        {
            Console.WriteLine(progressMessage.Output);
        }
    }
}
