using System;
using System.Runtime.Serialization;

namespace Sneal.CmdLineParser
{
    public class RequiredParameterMissingException : CmdLineParserException
    {
        public RequiredParameterMissingException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public RequiredParameterMissingException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public RequiredParameterMissingException(string message) : base(message)
        {
        }

        public RequiredParameterMissingException()
        {
        }
    }
}
