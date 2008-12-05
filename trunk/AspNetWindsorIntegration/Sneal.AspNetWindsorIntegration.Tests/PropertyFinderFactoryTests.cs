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
    public class PropertyFinderFactoryTests
    {
        [Test]
        public void Should_create_explicit_finder_when_behavior_is_explicit()
        {
            var finder = PropertyFinderFactory.Create(For.ExplicitProperties, typeof (object));
            Assert.IsTrue(typeof(ExplicitPropertyFinder) == finder.GetType());
        }

        [Test]
        public void Should_create_settable_finder_when_behavior_is_all()
        {
            var finder = PropertyFinderFactory.Create(For.AllProperties, typeof(object));
            Assert.IsTrue(typeof(SettablePropertyFinder) == finder.GetType());
        }

        [Test]
        public void Should_create_null_finder_when_behavior_is_none()
        {
            var finder = PropertyFinderFactory.Create(For.None, typeof(object));
            Assert.IsTrue(typeof(NullPropertyFinder) == finder.GetType());            
        }
    }
}
