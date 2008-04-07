using System;
using System.Runtime.Serialization;

namespace Sneal.CmdLineParser
{
    public class CmdLineParserException : ApplicationException
    {
        public CmdLineParserException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public CmdLineParserException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public CmdLineParserException(string message) : base(message)
        {
        }

        public CmdLineParserException()
        {
        }
    }
}