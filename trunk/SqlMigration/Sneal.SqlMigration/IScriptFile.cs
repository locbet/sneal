namespace Sneal.SqlMigration
{
    public interface IScriptFile
    {
        string Path { get; }
        bool IsXml { get; }
        bool IsSql { get; }
    }
}
