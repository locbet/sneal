using Autofac;
using NUnit.Framework;
using FluentNHibernate.Testing;
using Stormwind.Core.Models;
using Stormwind.Infrastructure.Data;

namespace Stormwind.IntegrationTests
{
    [TestFixture]
    public class ModelMappingTests
    {
        private IUnitOfWorkImplementor CurrentUow { get; set; }

        [Test]
        public void User_should_be_mapped()
        {
            using (NewContext())
            {
                new PersistenceSpecification<User>(CurrentUow.Session)
                    .CheckProperty(c => c.FirstName, "John")
                    .CheckProperty(c => c.LastName, "Doe")
                    .CheckProperty(c => c.EmailAddress, "jdoe@nowhere.com")
                    .VerifyTheMappings();
            }
        }

        private IContainer NewContext()
        {
            var ctx = AssemblySetup.ApplicationContainer.CreateInnerContainer();
            CurrentUow = ctx.Resolve<IUnitOfWorkImplementor>();
            CurrentUow.CommitMode = CommitMode.Explicit;
            return ctx;
        }
    }
}
