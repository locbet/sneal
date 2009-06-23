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

using System.Collections.Generic;
using NUnit.Framework;

namespace Sneal.CmdLineParser.Tests
{
    [TestFixture]
    public class DefaultUsageFormatterTests
    {
        private DefaultUsageFormatter _formatter = new DefaultUsageFormatter();

        [Test]
        public void Tab_length_should_default_to_20()
        {
            Assert.AreEqual(20, _formatter.DefaultMinTabSize);
        }

        [Test]
        public void Tab_length_should_adjust_to_accomodate_option_names_longer_than_20()
        {
            var options = new List<Option>
            {
                new Option { LongName = "ThisIsAReallyLongOptionOf32Chars", Description = "Some sort of string option" },
                new Option { ShortName = "a", Description = "A really short option" },
            };

            const string expected =
@"  /ThisIsAReallyLongOptionOf32Chars Some sort of string option
  /a                                A really short option
";
            string usage = _formatter.GetUsage(options);
            Assert.AreEqual(expected, usage);            
        }

        [Test]
        public void ShouldGetUsageLines()
        {
            var options = new List<Option>
            {
                new Option { LongName = "StringOption", Description = "Some sort of string option" },
                new Option { LongName = "BoolOption", Description = "Some sort of bool option" },
                new Option { LongName = "IntOption", Description = "Some sort of int option" },
                new Option { LongName = "StringList", Description = "Some sort of string collection option" }
            };

            const string expected =
@"  /StringOption     Some sort of string option
  /BoolOption       Some sort of bool option
  /IntOption        Some sort of int option
  /StringList       Some sort of string collection option
";
            string usage = _formatter.GetUsage(options);
            Assert.AreEqual(expected, usage);
        }

        [Test]
        public void Lines_longer_than_80chars_should_wrap_then_tab()
        {
            var options = new List<Option>
            {
                new Option { LongName = "aa", Description = "A really long description that wishes it could fit inside eighty chars, but it just can't because its so damn long." },
                new Option { ShortName = "b", Description = "A really short option" },
            };

            const string expected =
@"  /aa               A really long description that wishes it could fit inside
                    eighty chars, but it just can't because its so damn long.
  /b                A really short option
";
            string usage = _formatter.GetUsage(options);
            Assert.AreEqual(expected, usage);
        }

        [Test]
        public void Lines_longer_than_80chars_should_wrap_then_tab_even_with_extra_long_names()
        {
            var options = new List<Option>
            {
                new Option { LongName = "ThisIsAReallyLongOptionOf32Chars", Description = "A really long description that wishes it could fit inside eighty chars, but it just can't because its so damn long." },
                new Option { ShortName = "b", Description = "A really short option" },
            };

            const string expected =
@"  /ThisIsAReallyLongOptionOf32Chars A really long description that wishes it
                                    could fit inside eighty chars, but it just
                                    can't because its so damn long.
  /b                                A really short option
";
            string usage = _formatter.GetUsage(options);
            Assert.AreEqual(expected, usage);
        }

        [Test]
        public void Should_show_short_form_in_description_when_both_long_and_short_names_are_specified()
        {
            var options = new List<Option>
            {
                new Option { LongName="bob", ShortName = "b", Description = "A really short option" },
            };

            const string expected =
@"  /bob              A really short option (Short form: /b)
";
            string usage = _formatter.GetUsage(options);
            Assert.AreEqual(expected, usage);
        }
    }
}
