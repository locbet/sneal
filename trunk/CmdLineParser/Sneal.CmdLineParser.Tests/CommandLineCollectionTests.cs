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

using NUnit.Framework;

namespace Sneal.CmdLineParser.Tests
{
    [TestFixture]
    public class CommandLineCollectionTests
    {
        private CommandLineCollection _collection;

        [SetUp]
        public void SetUp()
        {
            _collection = new CommandLineCollection();
        }

        [Test]
        public void Should_add_option()
        {
            _collection.Add("option1=value1");
            Assert.AreEqual("value1", _collection.GetValue("option1"));
        }

        [Test]
        public void Should_add_option_with_different_case()
        {
            _collection.Add("option1=value1");
            Assert.AreEqual("value1", _collection.GetValue("OPTION1"));
        }

        [Test]
        public void Should_get_value_using_option_long_name()
        {
            var option = new Option {LongName = "option1"};
            _collection.Add("option1=value1");
            Assert.AreEqual("value1", _collection.GetValue(option));
        }

        [Test]
        public void Should_get_value_using_option_short_name()
        {
            var option = new Option { ShortName = "option1" };
            _collection.Add("option1=value1");
            Assert.AreEqual("value1", _collection.GetValue(option));
        }

        [Test]
        public void Contains_should_return_true_when_option_name_exists()
        {
            _collection.Add("option1=value1");
            Assert.IsTrue(_collection.Contains("option1"));
        }

        [Test]
        public void Contains_should_return_false_when_option_name_does_not_exist()
        {
            _collection.Add("option1=value1");
            Assert.IsFalse(_collection.Contains("option2"));
        }

        [Test]
        public void Contains_uses_short_name()
        {
            var option = new Option { ShortName = "option1" };
            _collection.Add("option1=value1");
            Assert.IsTrue(_collection.Contains(option));
        }

        [Test]
        public void Contains_uses_long_name()
        {
            var option = new Option { LongName = "option1" };
            _collection.Add("option1=value1");
            Assert.IsTrue(_collection.Contains(option));
        }
    }
}
