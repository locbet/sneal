using Castle.MicroKernel.Registration;
using Castle.Windsor;
using NUnit.Framework;

namespace Sneal.AspNetWindsorIntegration.Tests
{
    [TestFixture]
    public class AspNetDependencyBuilderTests
    {
        private WindsorContainer container;
        private AspNetDependencyBuilder dependencyBuilder;

        [SetUp]
        public void SetUp()
        {
            container = new WindsorContainer();
            dependencyBuilder = new AspNetDependencyBuilder(container);
        }

        [Test]
        public void Should_inject_dependency()
        {
            container.Register(Component.For<IService1>().ImplementedBy<Service1Impl>());

            var sut = new ClassWithDependency();
            Assert.IsNull(sut.Service1);

            dependencyBuilder.BuildUp(sut);

            Assert.IsNotNull(sut.Service1);
        }

        [Test]
        public void Should_inject_base_class_dependency()
        {
            container.Register(Component.For<IService1>().ImplementedBy<Service1Impl>());

            var sut = new DerivedClassWithDependency();
            Assert.IsNull(sut.Service1);

            dependencyBuilder.BuildUp(sut);

            Assert.IsNotNull(sut.Service1);
        }

        [Test]
        public void Should_inject_derived_class_dependency()
        {
            container.Register(Component.For<IService2>().ImplementedBy<Service2Impl>());

            var sut = new DerivedClassWithDependency();
            Assert.IsNull(sut.Service2);

            dependencyBuilder.BuildUp(sut);

            Assert.IsNotNull(sut.Service2);
        }

        [Test]
        public void Should_inject_class_dependency()
        {
            container.Register(Component.For<ServiceImpl>().ImplementedBy<ServiceImpl>());

            var sut = new DerivedClassWithDependency();
            Assert.IsNull(sut.ServiceImpl);

            dependencyBuilder.BuildUp(sut);

            Assert.IsNotNull(sut.ServiceImpl);
        }

        [Test]
        public void Should_inject_all_required_dependencies()
        {
            container.Register(Component.For<ServiceImpl>().ImplementedBy<ServiceImpl>());
            container.Register(Component.For<IService1>().ImplementedBy<Service1Impl>());
            container.Register(Component.For<IService2>().ImplementedBy<Service2Impl>());

            var sut = new DerivedClassWithDependency();

            dependencyBuilder.BuildUp(sut);

            Assert.IsNotNull(sut.Service1);
            Assert.IsNotNull(sut.Service2);
            Assert.IsNotNull(sut.ServiceImpl);
        }

        [Test]
        public void Should_not_throw_when_dependency_not_found()
        {
            var sut = new ClassWithDependency();
            Assert.IsNull(sut.Service1);

            dependencyBuilder.BuildUp(sut);

            Assert.IsNull(sut.Service1);
        }
    }

    public interface IService1 {}

    public interface IService2 {}

    public class Service1Impl : IService1 {}

    public class Service2Impl : IService2 { }

    public class ServiceImpl {}

    public class ClassWithDependency
    {
        private IService1 service1;

        public IService1 Service1
        {
            get { return service1; }
            set { service1 = value; }
        }
    }

    public class DerivedClassWithDependency : ClassWithDependency
    {
        private IService2 service2;
        private ServiceImpl serviceImpl;

        public IService2 Service2
        {
            get { return service2; }
            set { service2 = value; }
        }

        public ServiceImpl ServiceImpl
        {
            get { return serviceImpl; }
            set { serviceImpl = value; }
        }
    }
}
