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
        private Configuration _configuration;

        /// <summary>
        /// The underlying NHibernate Configuration instance.
        /// </summary>
        /// <remarks>
        /// This is null until you call RegisterNHibernateSessionFactory()
        /// and build the Autofac container, containerBuilder.Build().
        /// </remarks>
        public Configuration Configuration
        {
            get { return _configuration; }
        }

        public void RegisterNHibernateSessionFactory(ContainerBuilder containerBuilder, string connectionString)
        {
            containerBuilder.Register(x => CreateSessionFactory(connectionString))
                .As<ISessionFactory>()
                .SingletonScoped();
            containerBuilder.Register(x => x.Resolve<ISessionFactory>().OpenSession())
                .As<ISession>()
                .ContainerScoped();
        }

        protected virtual ISessionFactory CreateSessionFactory(string connectionString)
        {
            AutoPersistenceModel model = AutoMap.AssemblyOf<User>()
                .Where(t => t.Namespace == "Stormwind.Models");

            ISessionFactory sessionFactory = Fluently.Configure()
                .Database(MySQLConfiguration.Standard.ConnectionString(connectionString))
                .Mappings(m => m.AutoMappings.Add(model))
                // save this for later, SchemaExport needs it
                .ExposeConfiguration(cfg => _configuration = cfg)
                // MySql on Windows error.
                // http://stackoverflow.com/questions/1061128/mysql-hibernate-how-fix-the-error-column-reservedword-does-not-belong-to-ta
                .ExposeConfiguration(c => c.Properties.Add("hbm2ddl.keywords", "none"))
                .BuildSessionFactory();

            return sessionFactory;
        }
    }
}