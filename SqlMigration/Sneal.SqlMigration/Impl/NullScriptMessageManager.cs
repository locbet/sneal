namespace Sneal.SqlMigration.Impl
{
    /// <summary>
    /// Null object pattern implmentation for IScriptMessageManager.
    /// </summary>
    public class NullScriptMessageManager : IScriptMessageManager
    {
        public void OnScriptMessage(string msg) {}
    }
}
