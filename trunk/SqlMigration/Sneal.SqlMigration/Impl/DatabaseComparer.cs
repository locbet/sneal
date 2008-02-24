using MyMeta;

namespace Sneal.SqlMigration.Impl
{
    public class DatabaseComparer : IDatabaseComparer
    {
        protected delegate bool ExistsAction(IDatabase targetDB);

        protected ExistsAction existsAction;

        public IDatabaseComparer Table(ITable table)
        {
            existsAction = delegate(IDatabase targetDB) { return (targetDB.Tables[table.Name] != null); };

            return this;
        }

        public IDatabaseComparer Column(IColumn column)
        {
            existsAction = delegate(IDatabase targetDB)
                {
                    ITable table = targetDB.Tables[column.Table.Name];
                    if (table == null)
                        return false;

                    return (table.Columns[column.Name] != null);
                };

            return this;
        }

        public IDatabaseComparer ForeignKey(IForeignKey fk)
        {
            existsAction = delegate(IDatabase targetDB)
                {
                    ITable fkTable = targetDB.Tables[fk.ForeignTable.Name];
                    if (fkTable == null)
                        return false;

                    return (fkTable.ForeignKeys[fk.Name] != null);
                };

            return this;
        }

        public IDatabaseComparer Index(IIndex index)
        {
            existsAction = delegate(IDatabase targetDB)
                {
                    ITable indexTable = targetDB.Tables[index.Table.Name];
                    if (indexTable == null)
                        return false;

                    return (indexTable.Indexes[index.Name] != null);
                };

            return this;
        }

        public bool ExistsIn(IDatabase targetDB)
        {
            return existsAction(targetDB);
        }
    }
}