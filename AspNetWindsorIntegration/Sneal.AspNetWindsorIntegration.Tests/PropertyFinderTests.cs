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

using NUnit.Framework;

namespace Sneal.AspNetWindsorIntegration.Tests
{
    [TestFixture]
    public class PropertyFinderTests
    {
        [Test]
        public void Should_return_all_public_settable_properties()
        {
            SettablePropertyFinder finder = new SettablePropertyFinder(typeof(Employee));
            int propCount = 0;
            var e = finder.PropertiesToSet().GetEnumerator();
            while (e.MoveNext())
            {
                propCount++;
            }
            Assert.AreEqual(3, propCount);
        }

        [Test]
        public void Should_return_only_explicit_decorated_settable_properties()
        {
            ExplicitPropertyFinder finder = new ExplicitPropertyFinder(typeof(Employee));

            int propCount = 0;
            var e = finder.PropertiesToSet().GetEnumerator();
            while (e.MoveNext())
            {
                propCount++;
            }
            Assert.AreEqual(2, propCount);
        }

        [Test]
        public void Null_property_finder_should_return_no_properties()
        {
            NullPropertyFinder finder = new NullPropertyFinder(typeof(Employee));

            int propCount = 0;
            var e = finder.PropertiesToSet().GetEnumerator();
            while (e.MoveNext())
            {
                propCount++;
            }
            Assert.AreEqual(0, propCount);
        }
    }

    public class Person
    {
        [RequiredDependency]
        public string Name
        {
            get { return ""; }
            set { string s = value; }
        }

        [OptionalDependency]
        public string Address
        {
            get { return ""; }
            set { string s = value; }
        }
    }

    public class Employee : Person
    {
        public string Field;
        public object Data { get; set; }
        public int ID
        {
            get { return -1; }
        }
        public string StringMethod()
        {
            return "";
        }
    }
}
