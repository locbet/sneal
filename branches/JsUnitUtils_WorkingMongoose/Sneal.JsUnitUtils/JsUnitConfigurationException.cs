using System;
using System.Runtime.Serialization;

namespace Sneal.JsUnitUtils
{
    public class JsUnitConfigurationException : Exception
    {
        public JsUnitConfigurationException()
        {
        }

        public JsUnitConfigurationException(string message) : base(message)
        {
        }

        public JsUnitConfigurationException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected JsUnitConfigurationException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
