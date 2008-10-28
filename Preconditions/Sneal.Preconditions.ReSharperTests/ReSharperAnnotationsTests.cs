using System;
using System.Net;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using NUnit.Framework;
using JB = JetBrains.Annotations;
using Sneal.Preconditions.Aop;

namespace Sneal.Preconditions.ReSharperTests
{
    /// <summary>
    /// Tests the precondition facility using ReSharper NotNull attributes.
    /// </summary>
    [TestFixture]
    public class ReSharperAnnotationsTests
    {
        WindsorContainer container;
        private TestDomainObject testCustomer;

        [SetUp]
        public void SetUp()
        {
            container = new WindsorContainer();
            container.AddFacility("preconditions", new PreconditionFacility());
            container.Register(Component.For<TestDomainObject>());

            testCustomer = container.Resolve<TestDomainObject>();
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void StringNotNull_should_throw_with_null()
        {
            testCustomer.StringNotNull(null);
        }

        [Test]
        public void StringNotNull_should_not_throw_with_string()
        {
            testCustomer.StringNotNull("snealriffic");
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void IPAddressNotNull_should_throw_with_null()
        {
            testCustomer.IPAddressNotNull(null);
        }

        [Test]
        public void IPAddressNotNull_should_not_throw_with_ipaddress()
        {
            testCustomer.IPAddressNotNull(new IPAddress(0x2414188f));
        }

        [Test]
        public void String_should_not_throw_with_null_or_empty_string()
        {
            testCustomer.String(null);
            testCustomer.String("");
        }
    }
}
