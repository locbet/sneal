namespace Sneal.SqlMigration
{
    public interface IScriptWriter
    {
        string ExportDirectory { get; set; }

        IScriptMessageManager MessageManager { set; get; }

        void WriteIndexScript(string objectName, string fileContents);
        void WriteTableScript(string objectName, string fileContents);
        void WriteViewScript(string objectName, string fileContents);
        void WriteSprocScript(string objectName, string fileContents);
        void WriteTableDataScript(string objectName, string fileContents);
        void WriteForeignKeyScript(string objectName, string fileContents);
    }
}