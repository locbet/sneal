using System.Collections.Generic;

namespace Sneal.SqlMigration
{
    public interface IScriptingOptions
    {
        bool ScriptConstraints { get; }
        bool ScriptIndexes { get; }
        bool ScriptSchema { get; }
        bool ScriptData { get; }

        string ExportDirectory { get; }
        bool UseMultipleFiles { get; }

        IList<string> SprocsToScript { get; }
        IList<string> TablesToScript { get; }
        IList<string> ViewsToScript { get; }
    }
}