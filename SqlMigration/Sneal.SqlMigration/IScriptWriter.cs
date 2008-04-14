namespace Sneal.SqlMigration
{
    public interface IScriptWriter
    {
        string ExportDirectory { get; set; }

        IScriptMessageManager MessageManager { set; get; }

        void WriteIndexScript(string objectName, string sql);
        void WriteTableScript(string objectName, string sql);
        void WriteViewScript(string objectName, string sql);
        void WriteSprocScript(string objectName, string sql);
        void WriteTableDataScript(string objectName, string sql);
        void WriteForeignKeyScript(string objectName, string sql);
    }
}