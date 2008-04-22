namespace Sneal.SqlMigration.Migrators
{
    public class SqlServerBatchExecutor : BatchExecutor
    {
        public SqlServerBatchExecutor()
            : base("GO") {}

        public SqlServerBatchExecutor(IScriptMessageManager messageManager)
            : base("GO", messageManager) { }
    }
}
