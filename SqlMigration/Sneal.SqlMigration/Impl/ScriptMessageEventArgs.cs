using System;

namespace Sneal.SqlMigration.Impl
{
    public class ScriptMessageEventArgs : EventArgs
    {
        private readonly string message;

        public ScriptMessageEventArgs(string message)
        {
            this.message = message;
        }

        public string Message
        {
            get { return message; }
        }
    }
}