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
    public class RawOptionParserTests
    {
        private RawOptionParser _parser = new RawOptionParser();

        [Test]
        public void Should_split_name_and_value_at_equals_sign()
        {
            var pair = _parser.SplitOptionNameAndValue("option1=value1");
            Assert.AreEqual("option1", pair.Key);
            Assert.AreEqual("value1", pair.Value);
        }

        [Test]
        public void Should_split_ignoring_whitespace()
        {
            var pair = _parser.SplitOptionNameAndValue("   option1 = value1  ");
            Assert.AreEqual("option1", pair.Key);
            Assert.AreEqual("value1", pair.Value);
        }

        [Test]
        public void Should_strip_leading_dash_when_splitting()
        {
            var pair = _parser.SplitOptionNameAndValue("-option1=value1");
            Assert.AreEqual("option1", pair.Key);
            Assert.AreEqual("value1", pair.Value);
        }

        [Test]
        public void Should_strip_leading_slash_when_splitting()
        {
            var pair = _parser.SplitOptionNameAndValue("/option1=value1");
            Assert.AreEqual("option1", pair.Key);
            Assert.AreEqual("value1", pair.Value);
        }

        [Test]
        public void Value_should_be_null_when_not_specified()
        {
            var pair = _parser.SplitOptionNameAndValue("/option1");
            Assert.AreEqual("option1", pair.Key);
            Assert.IsNull(pair.Value);
        }

        [Test]
        public void Should_return_default_pair_when_option_name_and_value_are_empty()
        {
            var pair = _parser.SplitOptionNameAndValue("");
            Assert.IsNull(pair.Key);
            Assert.IsNull(pair.Value);
        }

        [Test]
        public void Should_return_default_pair_when_option_name_and_value_are_null()
        {
            var pair = _parser.SplitOptionNameAndValue(null);
            Assert.IsNull(pair.Key);
            Assert.IsNull(pair.Value);
        }

        [Test]
        public void Should_strip_leading_slash()
        {
            Assert.AreEqual("option1", _parser.StripSwitchChar("/option1"));
        }

        [Test]
        public void Should_strip_leading_dash()
        {
            Assert.AreEqual("option1", _parser.StripSwitchChar("-option1"));
        }

        [Test]
        public void Should_strip_leading_whitespace_and_slash()
        {
            Assert.AreEqual("option1", _parser.StripSwitchChar(" /option1"));
        }

        [Test]
        public void StripSwitchChar_should_return_empty_string_when_passed_null()
        {
            Assert.AreEqual("", _parser.StripSwitchChar(null));
        }
    }
}
