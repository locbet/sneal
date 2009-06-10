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
    public class BooleanPropertySetterTests
    {
        private BooleanPropertyStub _booleanStub;
        private BooleanPropertySetter _booleanSetter;

        [SetUp]
        public void SetUp()
        {
            _booleanStub = new BooleanPropertyStub();
            _booleanSetter = new BooleanPropertySetter();
        }

        [Test]
        public void Should_parse_true()
        {
            _booleanSetter.SetPropertyValue(
                new Option(),
                _booleanStub.GetPropertyInfo(),
                _booleanStub,
                "true");

            Assert.IsTrue(_booleanStub.BooleanProperty);
        }

        [Test]
        public void Should_parse_one_as_true()
        {
            _booleanSetter.SetPropertyValue(
                new Option(),
                _booleanStub.GetPropertyInfo(),
                _booleanStub,
                "1");

            Assert.IsTrue(_booleanStub.BooleanProperty);
        }

        [Test]
        public void Should_default_to_true_when_value_is_null()
        {
            _booleanSetter.SetPropertyValue(
                new Option(),
                _booleanStub.GetPropertyInfo(),
                _booleanStub,
                null);

            Assert.IsTrue(_booleanStub.BooleanProperty);
        }

        [Test]
        public void Should_default_to_true_when_value_is_empty()
        {
            _booleanSetter.SetPropertyValue(
                new Option(),
                _booleanStub.GetPropertyInfo(),
                _booleanStub,
                "");

            Assert.IsTrue(_booleanStub.BooleanProperty);
        }

        [Test]
        [ExpectedException(typeof(CommandLineException))]
        public void Should_throw_exception_when_non_true_false()
        {
            _booleanSetter.SetPropertyValue(
                new Option(),
                _booleanStub.GetPropertyInfo(),
                _booleanStub,
                "a string");
        }
    }

    public class BooleanPropertyStub
    {
        public bool BooleanProperty { get; set; }

        public PropertyInfo GetPropertyInfo()
        {
            return typeof(BooleanPropertyStub).GetProperties()[0];
        }
    }
}
