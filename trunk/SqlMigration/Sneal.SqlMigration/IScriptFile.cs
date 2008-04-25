using System.IO;

namespace Sneal.SqlMigration
{
    public interface IScriptFile
    {
        string Path { get; }
        bool IsXml { get; }
        bool IsSql { get; }
        string GetContent();
        Stream GetContentStream();
    }
}
