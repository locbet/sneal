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
    public class StringPropertySetterTests
    {
        private StringPropertyStub _stringStub;
        private StringPropertySetter _stringSetter;

        [SetUp]
        public void SetUp()
        {
            _stringStub = new StringPropertyStub();
            _stringSetter = new StringPropertySetter();
        }

        [Test]
        public void Should_parse_alpha_string()
        {
            _stringSetter.SetPropertyValue(
                new Option(),
                _stringStub.GetPropertyInfo(),
                _stringStub,
                "abcdefg");

            Assert.AreEqual("abcdefg", _stringStub.StringProperty);
        }

        [Test]
        public void Should_use_null_when_value_is_null()
        {
            _stringSetter.SetPropertyValue(
                new Option(),
                _stringStub.GetPropertyInfo(),
                _stringStub,
                null);

            Assert.AreEqual(null, _stringStub.StringProperty);
        }

        [Test]
        public void Should_use_empty_string_when_value_is_empty()
        {
            _stringSetter.SetPropertyValue(
                new Option(),
                _stringStub.GetPropertyInfo(),
                _stringStub,
                "");

            Assert.AreEqual("", _stringStub.StringProperty);
        }
    }

    public class StringPropertyStub
    {
        public string StringProperty { get; set; }

        public PropertyInfo GetPropertyInfo()
        {
            return typeof(StringPropertyStub).GetProperties()[0];
        }
    }
}
