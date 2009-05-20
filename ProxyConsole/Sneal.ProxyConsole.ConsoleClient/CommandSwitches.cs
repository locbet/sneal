using Sneal.CmdLineParser;

namespace Sneal.ProxyConsole.ConsoleClient
{
    public class CommandSwitches
    {
        private string _executableFileName;
        private string _arguments;
        private string _workingDirectory;
        private string _zipFile;
        private string _endpointAddress;
        private int _timeout;

        [Switch(
            Name = "ConsoleAddress",
            Description = "The endpoint address of the remote ConsoleHost to send remote commands to",
            Required = true)]
        public string EndpointAddress
        {
            get { return _endpointAddress; }
            set { _endpointAddress = value; }
        }

        [Switch(
            Name = "Executable",
            Description = "The executable file name to run, for example: ping.exe",
            Required = true)]
        public string ExecutableFileName
        {
            get { return _executableFileName; }
            set { _executableFileName = value; }
        }

        [Switch(
            Name = "Arguments",
            Description = "The optional command line arguments to pass to Executable")]
        public string Arguments
        {
            get { return _arguments; }
            set { _arguments = value; }
        }

        [Switch(
            Name = "WorkingDirectory",
            Description = "The working directory to run Executable from, this is relative to the remote machine")]
        public string WorkingDirectory
        {
            get { return _workingDirectory; }
            set { _workingDirectory = value; }
        }

        [Switch(
            Name = "ZipFile",
            Description = "The path to the local zip file to attach")]
        public string ZipFile
        {
            get { return _zipFile; }
            set { _zipFile = value; }
        }

        [Switch(
            Name = "Timeout",
            Description = "Time in seconds to wait for the remote process to finish")]
        public int Timeout
        {
            get { return _timeout; }
            set { _timeout = value; }
        }
    }
}
