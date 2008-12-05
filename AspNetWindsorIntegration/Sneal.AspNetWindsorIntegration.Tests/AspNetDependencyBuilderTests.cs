#region license
// Copyright 2008 Shawn Neal (sneal@sneal.net)
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion

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

            var sut = new ClassWithExplcitDependencies();
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

        [Test]
        public void Should_only_use_top_level_class_attribute()
        {
            var attrs = typeof (DerivedClassWithDependency)
                .GetCustomAttributes(typeof (UsesInjectionAttribute), true);
            Assert.AreEqual(1, attrs.Length);
            var usesInjection = (UsesInjectionAttribute)attrs[0];
            Assert.AreEqual(For.AllProperties, usesInjection.Behavior);
        }
    }

    public interface IService1 {}

    public interface IService2 {}

    public class Service1Impl : IService1 {}

    public class Service2Impl : IService2 { }

    public class ServiceImpl {}

    [UsesInjection(For.None)]
    public class ClassWithDependency
    {
        private IService1 service1;

        public IService1 Service1
        {
            get { return service1; }
            set { service1 = value; }
        }
    }

    [UsesInjection(For.AllProperties)]
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

    [UsesInjection(For.ExplicitProperties)]
    public class ClassWithExplcitDependencies
    {
        private IService2 service2;
        private IService1 service1;

        public IService2 Service2
        {
            get { return service2; }
            set { service2 = value; }
        }

        [RequiredDependency]
        public IService1 Service1
        {
            get { return service1; }
            set { service1 = value; }
        }
    }
}
