#region license
// Copyright 2009 Shawn Neal (neal.shawn@gmail.com)
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
using Sneal.CmdLineParser.PropertySetters;

namespace Sneal.CmdLineParser.Tests.PropertySetters
{
    [TestFixture]
    public class IntegerPropertySetterTests
    {
        private IntPropertyStub _intStub;
        private IntegerPropertySetter _intSetter;

        [SetUp]
        public void SetUp()
        {
            _intStub = new IntPropertyStub();
            _intSetter = new IntegerPropertySetter();
        }

        [Test]
        public void Should_parse_integer()
        {
            _intSetter.SetPropertyValue(
                new Option(),
                _intStub.GetPropertyInfo(),
                _intStub,
                "12");

            Assert.AreEqual(12, _intStub.IntProperty);
        }

        [Test]
        public void Should_default_to_zero_when_value_is_null()
        {
            _intSetter.SetPropertyValue(
                new Option(),
                _intStub.GetPropertyInfo(),
                _intStub,
                null);

            Assert.AreEqual(0, _intStub.IntProperty);
        }

        [Test]
        public void Should_default_to_zero_when_value_is_empty()
        {
            _intSetter.SetPropertyValue(
                new Option(),
                _intStub.GetPropertyInfo(),
                _intStub,
                "");

            Assert.AreEqual(0, _intStub.IntProperty);
        }

        [Test]
        [ExpectedException(typeof(CommandLineException))]
        public void Should_throw_exception_when_non_digit()
        {
            _intSetter.SetPropertyValue(
                new Option(),
                _intStub.GetPropertyInfo(),
                _intStub,
                "a string");
        }
    }

    public class IntPropertyStub
    {
        public int IntProperty { get; set; }

        public PropertyInfo GetPropertyInfo()
        {
            return typeof(IntPropertyStub).GetProperties()[0];
        }
    }
}
