using MyMeta;

namespace Sneal.SqlMigration
{
    public interface IScriptBuilder
    {
        SqlScript Create(ITable table);
        SqlScript Drop(ITable table);

        SqlScript Create(IColumn column);
        SqlScript Drop(IColumn column);

        SqlScript Create(IIndex index);
        SqlScript Drop(IIndex index);

        SqlScript Create(IForeignKey fk);
        SqlScript Drop(IForeignKey fk);

        SqlScript Create(IProcedure sproc);
        SqlScript Drop(IProcedure sproc);
        SqlScript Alter(IProcedure sproc);

        SqlScript Create(IView view);
        SqlScript Drop(IView view);
        SqlScript Alter(IView view);
        SqlScript Alter(IColumn column);
    }
}