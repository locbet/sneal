using System;
using System.Threading;
using NUnit.Framework;
using Rhino.Mocks;
using Sneal.ProxyConsole.WcfService;

namespace Sneal.ProxyConsole.Tests
{
    [TestFixture]
    public class ConsoleRunnerTests
    {
        private string _output = "";
        private ExecutionFinishedMessage _finishMsg;
        private ExecutionProgressMessage _progressMsg;
        private bool _isComplete;

        [Test]
        public void Should_send_output_async()
        {
            IConsoleListener listener = MockRepository.GenerateMock<IConsoleListener>();
            listener.Stub(o => o.ExecutionProgress(null)).IgnoreArguments().WhenCalled(a => GetExecutionProgressMessage(a.Arguments[0]));
            listener.Stub(o => o.ExecutionComplete(null)).IgnoreArguments().WhenCalled(a => GetExecutionFinishedMessage(a.Arguments[0]));

            StartExecutionMessage executionMessage = new StartExecutionMessage("ping.exe");
            executionMessage.Arguments = "localhost";
            ConsoleRunner runner = new ConsoleRunner(listener);
            runner.Run(executionMessage);

            WaitForCompletionMessage();

            Assert.That(_output.Contains("Pinging"));
            Assert.That(_output.Contains("Approximate round trip times in milli-seconds"));
        }

        [Test]
        public void Should_send_exit_code()
        {
            IConsoleListener listener = MockRepository.GenerateMock<IConsoleListener>();
            listener.Stub(o => o.ExecutionProgress(null)).IgnoreArguments().WhenCalled(a => GetExecutionProgressMessage(a.Arguments[0]));
            listener.Stub(o => o.ExecutionComplete(null)).IgnoreArguments().WhenCalled(a => GetExecutionFinishedMessage(a.Arguments[0]));

            StartExecutionMessage executionMessage = new StartExecutionMessage("ping.exe");
            executionMessage.Arguments = "localhost";
            ConsoleRunner runner = new ConsoleRunner(listener);
            runner.Run(executionMessage);

            WaitForCompletionMessage();

            Assert.AreEqual(0, _finishMsg.ExitCode, "The exit code was an error, i.e. non-zero");
            Assert.IsFalse(_finishMsg.IsError, "The response was an error");
        }

        private void WaitForCompletionMessage()
        {
            while (!_isComplete)
            {
                Thread.Sleep(100);
            }
        }

        private void GetExecutionFinishedMessage(object executionFinishedMessage)
        {
            _isComplete = true;
            _finishMsg = (ExecutionFinishedMessage)executionFinishedMessage;
            Console.WriteLine(_finishMsg.Output);
        }

        private void GetExecutionProgressMessage(object executionProgressMessage)
        {
            _progressMsg = (ExecutionProgressMessage) executionProgressMessage;
            _output += _progressMsg.Output;
            Console.WriteLine(_progressMsg.Output);
        }
    }
}
