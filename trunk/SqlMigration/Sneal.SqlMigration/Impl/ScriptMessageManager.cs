using System;

namespace Sneal.SqlMigration.Impl
{
    /// <summary>
    /// This class handles messaging for the migration engine.
    /// </summary>
    public class ScriptMessageManager : IScriptMessageManager
    {
        public event EventHandler<ScriptMessageEventArgs> ScriptMessage;

        public virtual void OnScriptMessage(string msg)
        {
            EventHandler<ScriptMessageEventArgs> handler = ScriptMessage;
            if (handler != null)
                handler(this, new ScriptMessageEventArgs(msg));
        }
    }
}
