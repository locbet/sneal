using Autofac.Builder;
using FluentNHibernate.Automapping;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using NHibernate;
using NHibernate.Cfg;
using Stormwind.Models;

namespace Stormwind.Infrastructure
{
    public class NHibernateConfiguration
    {
        /// <summary>
        /// The underlying NHibernate Configuration instance.
        /// </summary>
        /// <remarks>
        /// This is null until you call RegisterNHibernateSessionFactory()
        /// and build the Autofac container, containerBuilder.Build().
        /// </remarks>
        public Configuration Configuration { get; private set; }

        public void RegisterMySqlSessionFactory(ContainerBuilder containerBuilder, string connectionString)
        {
            containerBuilder.Register(x => CreateMySqlSessionFactory(connectionString))
                .As<ISessionFactory>()
                .SingletonScoped();
            containerBuilder.Register(x => x.Resolve<ISessionFactory>().OpenSession())
                .As<ISession>()
                .ContainerScoped();
        }

        public void RegisterSqlServerSessionFactory(ContainerBuilder containerBuilder, string connectionString)
        {
            containerBuilder.Register(x => CreateSqlServerSessionFactory(connectionString))
                .As<ISessionFactory>()
                .SingletonScoped();
            containerBuilder.Register(x => x.Resolve<ISessionFactory>().OpenSession())
                .As<ISession>()
                .ContainerScoped();
        }

        private AutoPersistenceModel GetAutoPersistenceModel()
        {
            return AutoMap.AssemblyOf<User>().Where(t => t.Namespace == "Stormwind.Models");
        }

        protected virtual ISessionFactory CreateMySqlSessionFactory(string connectionString)
        {
            return Fluently.Configure()
                .Database(MySQLConfiguration.Standard.ConnectionString(connectionString))
                .Mappings(m => m.AutoMappings.Add(GetAutoPersistenceModel()))
                // save this for later, SchemaExport needs it
                .ExposeConfiguration(cfg => Configuration = cfg)
                // MySql on Windows error.
                // http://stackoverflow.com/questions/1061128/mysql-hibernate-how-fix-the-error-column-reservedword-does-not-belong-to-ta
                .ExposeConfiguration(c => c.Properties.Add("hbm2ddl.keywords", "none"))
                .BuildSessionFactory();
        }

        protected virtual ISessionFactory CreateSqlServerSessionFactory(string connectionString)
        {
            return Fluently.Configure()
                .Database(MsSqlConfiguration.MsSql2005.ConnectionString(connectionString))
                .Mappings(m => m.AutoMappings.Add(GetAutoPersistenceModel()))
                // save this for later, SchemaExport needs it
                .ExposeConfiguration(cfg => Configuration = cfg)
                .BuildSessionFactory();
        }
    }
}