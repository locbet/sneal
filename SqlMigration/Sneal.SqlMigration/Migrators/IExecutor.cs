using MyMeta;

namespace Sneal.SqlMigration.Migrators
{
    /// <summary>
    /// Represents an object that can be used to run a script or other file
    /// based action against an RDMS system.  Generally an IExecutor will
    /// be used to run SQL scripts against a database.
    /// </summary>
    public interface IExecutor
    {
        /// <summary>
        /// Executes the specified scriptfile against the db.
        /// </summary>
        /// <param name="db">The db to run the script against.</param>
        /// <param name="scriptFile">The script file instance to execute.</param>
        void Execute(IDatabase db, IScriptFile scriptFile);
    }
}
