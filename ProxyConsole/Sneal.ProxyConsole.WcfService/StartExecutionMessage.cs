using System.Runtime.Serialization;

namespace Sneal.ProxyConsole.WcfService
{
    [DataContract]
    public class StartExecutionMessage
    {
        private string _fileName;
        private string _arguments;
        private string _workingDirectory;
        private int _timeout;
        private byte[] _zipPackage;

        public StartExecutionMessage()
        {
        }

        public StartExecutionMessage(string fileName)
        {
            _fileName = fileName;
        }

        [DataMember]
        public string FileName
        {
            get { return _fileName; }
            set { _fileName = value; }
        }

        [DataMember]
        public string Arguments
        {
            get { return _arguments; }
            set { _arguments = value; }
        }

        [DataMember]
        public string WorkingDirectory
        {
            get { return _workingDirectory; }
            set { _workingDirectory = value; }
        }

        [DataMember]
        public int Timeout
        {
            get { return _timeout; }
            set { _timeout = value; }
        }

        [DataMember]
        public byte[] ZipPackage
        {
            get { return _zipPackage; }
            set { _zipPackage = value; }
        }
    }
}
