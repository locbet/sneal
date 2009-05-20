using System;
using System.Diagnostics;
using System.ServiceModel;
using System.Threading;

namespace Sneal.ProxyConsole.WcfService
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
    public class ConsoleRunner : IConsoleRunner
    {
        private IConsoleListener _consoleListener;
        private Thread _consoleThread;
        private StartExecutionMessage _requestMessage;
        public event EventHandler<DataReceivedEventArgs> ConsoleDataReceived;

        public ConsoleRunner()
        {
        }

        public ConsoleRunner(IConsoleListener consoleListener)
        {
            _consoleListener = consoleListener;
        }

        public IConsoleListener ConsoleListener
        {
            get { return _consoleListener; }
        }

        #region IConsoleRunner Members

        public void Run(StartExecutionMessage requestMsg)
        {
            ConfigureSessionContext(requestMsg);
            UnzipOptionalPackage();
            ExecuteConsoleCommandAsync();
        }

        #endregion

        private void ConfigureSessionContext(StartExecutionMessage requestMsg)
        {
            _requestMessage = requestMsg;
            if (_consoleListener == null)
                _consoleListener = OperationContext.Current.GetCallbackChannel<IConsoleListener>();
        }

        private void UnzipOptionalPackage()
        {
            if (_requestMessage.ZipPackage == null)
                return;

            // TODO: unzip
        }

        private void ExecuteConsoleCommandAsync()
        {
            _consoleThread = new Thread(ExecuteConsole);
            _consoleThread.Start();
        }

        private void ExecuteConsole()
        {
            try
            {
                int exitCode = RunProcess();
                _consoleListener.ExecutionComplete(new ExecutionFinishedMessage(exitCode));
            }
            catch (Exception ex)
            {
                _consoleListener.ExecutionComplete(new ExecutionFinishedMessage
                {
                    ExitCode = -1,
                    IsError = true,
                    Output = ex.ToString()
                });
            }
        }

        private int RunProcess()
        {
            using (Process process = CreateProcessInstance())
            {
                process.OutputDataReceived += ProcessOutputDataReceived;
                process.Start();
                process.BeginOutputReadLine();

                WaitForProcessExit(process);
                return process.ExitCode;
            }
        }

        private Process CreateProcessInstance()
        {
            return new Process
            {
                StartInfo =
                    {
                        FileName = _requestMessage.FileName,
                        Arguments = _requestMessage.Arguments,
                        WorkingDirectory = _requestMessage.WorkingDirectory,
                        UseShellExecute = false,
                        RedirectStandardOutput = true
                    }
            };
        }

        private void WaitForProcessExit(Process process)
        {
            if (_requestMessage.Timeout > 0)
                process.WaitForExit(_requestMessage.Timeout);
            else
                process.WaitForExit();
        }

        private void ProcessOutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (string.IsNullOrEmpty(e.Data))
                return;

            var evt = ConsoleDataReceived;
            if (evt != null)
            {
                evt(this, e);
            }
            _consoleListener.ExecutionProgress(new ExecutionProgressMessage(e.Data));
        }
    }
}