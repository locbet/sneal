using System;
using System.Diagnostics;
using System.ServiceModel;
using System.Threading;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Sneal.ProxyConsole.Tests.ConsoleRunnerService;
using Sneal.ProxyConsole.WcfService;

namespace Sneal.ProxyConsole.Tests
{
    [TestFixture]
    public class ConsoleHostTests
    {
        private Process _host;

        [Test, Explicit]
        public void Should_call_remote_console()
        {
            StartConsoleHost();

            var testHandler = new CallbackHandler();
            var svcProxy = new ConsoleRunnerClient(new InstanceContext(testHandler));
            var startMsg = new StartExecutionMessage("ping.exe");
            startMsg.Arguments = "localhost";
            svcProxy.Run(startMsg);

            // wait for completion
            while (!testHandler.IsComplete)
            {
                Thread.Sleep(100);
            }
        }

        private void StartConsoleHost()
        {
            _host = new Process();
            _host.StartInfo.FileName = "Sneal.ProxyConsole.ConsoleHost.exe";
            _host.StartInfo.WorkingDirectory = AppDomain.CurrentDomain.BaseDirectory +
                                              @"\..\..\..\Sneal.ProxyConsole.ConsoleHost\bin\debug";

            _host.Start();
        }

        [TearDown]
        public void Dispose()
        {
            _host.CloseMainWindow();
        }
    }

    public class CallbackHandler : ConsoleRunnerCallback
    {
        public bool IsComplete;

        public void ExecutionComplete(ExecutionFinishedMessage completeMessage)
        {
            IsComplete = true;
            Assert.IsFalse(completeMessage.IsError);
            Assert.That(completeMessage.ExitCode, Is.GreaterThanOrEqualTo(0));
        }

        public void ExecutionProgress(ExecutionProgressMessage progressMessage)
        {
            Console.WriteLine(progressMessage.Output);
        }
    }
}
