using System;
using System.Text;

namespace Sneal.ReSharper.MsTest
{
    /// <summary>
    /// Adapts a MSTest exception, stripping out any lines from the stack trace
    /// that originate from MSTest.
    /// </summary>
    public class MSTestExceptionAdapter : Exception
    {
        private Exception exception;

        public MSTestExceptionAdapter(Exception exception)
        {
            this.exception = exception;
        }

        public override string Message
        {
            get { return exception.Message; }
        }

        public override string Source
        {
            get { return exception.Source; }
            set { exception.Source = value; }
        }

        public override string StackTrace
        {
            get { return GetStackTraceWithoutMSTestNS(); }
        }

        public override System.Collections.IDictionary Data
        {
            get { return exception.Data; }
        }

        public override string ToString()
        {
            return exception.ToString();
        }

        public override string HelpLink
        {
            get { return exception.HelpLink; }
            set { exception.HelpLink = value; }
        }
        
        /// <summary>
        /// Gets the cleaned up stack trace.
        /// </summary>
        private string GetStackTraceWithoutMSTestNS()
        {
            StringBuilder newStackTrace = new StringBuilder();
            string[] lines = exception.StackTrace.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string line in lines)
            {
                if (!line.Contains("Microsoft.VisualStudio.TestTools.UnitTesting"))
                {
                    newStackTrace.AppendLine(line);
                }
            }
            return newStackTrace.ToString();
        }

        public override bool Equals(object obj)
        {
            return exception.Equals(obj);
        }

        public override int GetHashCode()
        {
            return exception.GetHashCode();
        }
    }
}
