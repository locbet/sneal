using MyMeta;

namespace Sneal.SqlMigration.Migrators
{
    public interface IDataMigrator
    {
        SqlScript ScriptAllData(ITable sourceTable, SqlScript script);
    }
}
