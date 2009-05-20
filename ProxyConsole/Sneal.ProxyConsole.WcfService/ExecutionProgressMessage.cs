using System.Runtime.Serialization;

namespace Sneal.ProxyConsole.WcfService
{
    [DataContract]
    public class ExecutionProgressMessage
    {
        private string _output;

        public ExecutionProgressMessage()
        {
        }

        public ExecutionProgressMessage(string output)
        {
            _output = output;
        }

        [DataMember]
        public string Output
        {
            get { return _output; }
            set { _output = value; }
        }
    }
}
