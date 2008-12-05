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

using System.Reflection;
using NUnit.Framework;

namespace Sneal.AspNetWindsorIntegration.Tests
{
    [TestFixture]
    public class PropertyTests
    {
        [Test]
        public void Should_be_required_dependency()
        {
            var property = GetProperty("Street");
            Assert.IsTrue(property.IsRequiredDependency);
            Assert.IsFalse(property.IsOptionalDependency);
        }

        [Test]
        public void Should_be_optional_dependency()
        {
            var property = GetProperty("Age");
            Assert.IsFalse(property.IsRequiredDependency);
            Assert.IsTrue(property.IsOptionalDependency);            
        }

        [Test]
        public void Should_set_property_value()
        {
            var address = new Address();
            var property = GetProperty("Age");
            property.SetValue(address, 16);
            Assert.AreEqual(16, address.Age);
        }

        private static Property GetProperty(string properyName)
        {
            return new Property(
                typeof (Address).GetProperty(
                    properyName,
                    BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                );
        }
    }

    public class Address
    {
        private string street;
        private int age;

        [RequiredDependency]
        public string Street
        {
            get { return street; }
            set { street = value; }
        }

        [OptionalDependency]
        public int Age
        {
            get { return age; }
            set { age = value; }
        }
    }
}
