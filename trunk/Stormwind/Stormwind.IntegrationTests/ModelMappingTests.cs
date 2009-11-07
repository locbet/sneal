using NUnit.Framework;
using FluentNHibernate.Testing;
using Microsoft.Practices.ServiceLocation;
using NHibernate;
using Stormwind.Models;

namespace Stormwind.IntegrationTests
{
    [TestFixture]
    public class ModelMappingTests
    {
        [Test]
        public void User_should_be_mapped()
        {
            using (Session.BeginTransaction())
            {
                new PersistenceSpecification<User>(Session)
                    .CheckProperty(c => c.FirstName, "John")
                    .CheckProperty(c => c.LastName, "Doe")
                    .CheckProperty(c => c.EmailAddress, "jdoe@nowhere.com")
                    .VerifyTheMappings();
            }
        }

        public ISession Session
        {
            get { return ServiceLocator.Current.GetInstance<ISession>(); }
        }
    }
}
