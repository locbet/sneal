using System.Runtime.Serialization;

namespace Sneal.ProxyConsole.WcfService
{
    [DataContract]
    public class ExecutionFinishedMessage
    {
        private string _output;
        private int _exitCode;
        private bool _isError;

        public ExecutionFinishedMessage()
        {
        }

        public ExecutionFinishedMessage(int exitCode)
        {
            _exitCode = exitCode;
        }

        [DataMember]
        public string Output
        {
            get { return _output; }
            set { _output = value; }
        }

        [DataMember]
        public int ExitCode
        {
            get { return _exitCode; }
            set { _exitCode = value; }
        }

        [DataMember]
        public bool IsError
        {
            get { return _isError; }
            set { _isError = value; }
        }
    }
}
