using System.Collections.Generic;

namespace Sneal.SqlMigration
{
    public interface IScriptingOptions
    {
        bool ScriptForeignKeys { get; }
        bool ScriptIndexes { get; }
        bool ScriptSchema { get; }
        bool ScriptData { get; }
        bool ScriptDataAsXml { get; }

        string ExportDirectory { get; }
        bool UseMultipleFiles { get; }

        IList<DbObjectName> SprocsToScript { get; }
        IList<DbObjectName> TablesToScript { get; }
        IList<DbObjectName> ViewsToScript { get; }
    }
}