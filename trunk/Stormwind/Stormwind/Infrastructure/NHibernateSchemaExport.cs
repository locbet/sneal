using NHibernate;
using NHibernate.Cfg;
using NHibernate.Tool.hbm2ddl;

namespace Stormwind.Infrastructure
{
    public class NHibernateSchemaExport
    {
        public void ExportNHibernateSchema(ISession session, Configuration nhibernateConfiguration)
        {
            var export = new SchemaExport(nhibernateConfiguration);
            export.Execute(true, true, false, session.Connection, null);
        }
    }
}