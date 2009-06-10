#region license
// Copyright 2008 Shawn Neal (neal.shawn@gmail.com)
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
    public class CommandLineParserTests
    {
        private TestOptions testOptions;
        private RequiredTestOptions requiredTestOptions;
        private CommandLineParser parser;

        [SetUp]
        public void SetUp()
        {
            testOptions = new TestOptions();
            requiredTestOptions = new RequiredTestOptions();
        }

        [Test]
        public void ShouldGetSettableProperties()
        {
            parser = new CommandLineParser();

            List<Option> settableOptions = parser.GetSettableOptions(testOptions);

            Assert.IsNotNull(settableOptions);
            Assert.That(settableOptions.Count == 4);

            Assert.AreEqual("StringOption", settableOptions[0].Name);
            Assert.AreEqual("BoolOption", settableOptions[1].Name);
            Assert.AreEqual("IntOption", settableOptions[2].Name);
            Assert.AreEqual("StringList", settableOptions[3].Name);
        }

        [Test]
        public void Boolean_option_should_be_false_when_not_specified()
        {
            parser = new CommandLineParser("/StringOption=Sneal /IntOption=22");
            testOptions = parser.BuildOptions(testOptions);
            Assert.IsFalse(testOptions.BoolOption);
        }

        [Test]
        public void Should_set_options_using_long_names()
        {
            parser = new CommandLineParser("/StringOption=Sneal /BoolOption /IntOption=22");

            testOptions = parser.BuildOptions(testOptions);

            Assert.AreEqual("Sneal", testOptions.StringOption);
            Assert.IsTrue(testOptions.BoolOption);
            Assert.AreEqual(22, testOptions.IntOption);
        }

        [Test]
        public void Should_set_options_using_short_names()
        {
            parser = new CommandLineParser("/s=Sneal /b /i=22");

            testOptions = parser.BuildOptions(testOptions);

            Assert.AreEqual("Sneal", testOptions.StringOption);
            Assert.IsTrue(testOptions.BoolOption);
            Assert.AreEqual(22, testOptions.IntOption);
        }

        [Test]
        public void Should_ignore_extra_spaces()
        {
            parser = new CommandLineParser("  /StringOption=Sneal  /BoolOption  /IntOption=22 ");

            testOptions = parser.BuildOptions(testOptions);

            Assert.AreEqual("Sneal", testOptions.StringOption);
            Assert.IsTrue(testOptions.BoolOption);
            Assert.AreEqual(22, testOptions.IntOption);
        }

        [Test]
        public void Should_work_with_dashes()
        {
            parser = new CommandLineParser("-StringOption=Sneal");
            testOptions = parser.BuildOptions(testOptions);
            Assert.AreEqual("Sneal", testOptions.StringOption);
        }

        [Test]
        public void Can_explcitly_set_boolean_option()
        {
            parser = new CommandLineParser("/BoolOption=true");
            testOptions = parser.BuildOptions(testOptions);
            Assert.IsTrue(testOptions.BoolOption);
        }

        [Test]
        public void Can_implcitly_set_boolean_option()
        {
            parser = new CommandLineParser("/BoolOption");
            testOptions = parser.BuildOptions(testOptions);
            Assert.IsTrue(testOptions.BoolOption);
        }

        [Test]
        public void Should_not_require_value_for_string()
        {
            parser = new CommandLineParser("/StringOption");
            parser.BuildOptions(testOptions);
        }

        [Test]
        public void Should_not_require_value_for_int()
        {
            parser = new CommandLineParser("/IntOption");
            parser.BuildOptions(testOptions);
        }

        [Test]
        public void Can_new_up_parser_with_empty_ctor()
        {
            new CommandLineParser();
        }

        [Test]
        public void ShouldGetUsageLines()
        {
            const string expected =
@"StringOption         Some sort of string option
BoolOption           Some sort of bool option
IntOption            Some sort of int option
StringList           Some sort of string collection option
";
            parser = new CommandLineParser();
            string usage = parser.GetUsageLines(testOptions);
            Assert.AreEqual(expected, usage);
        }

        [Test]
        public void Should_return_true_if_a_required_param_is_missing()
        {
            // missing required int option
            parser = new CommandLineParser("/StringOption=Sneal /BoolOption");
            requiredTestOptions = parser.BuildOptions(requiredTestOptions);
            Assert.IsTrue(parser.IsMissingRequiredOptions(requiredTestOptions));
        }

        [Test]
        public void Should_return_false_if_an_optional_param_is_missing()
        {
            // missing optional int option
            parser = new CommandLineParser("/StringOption=Sneal /BoolOption");
            testOptions = parser.BuildOptions(testOptions);
            Assert.IsFalse(parser.IsMissingRequiredOptions(testOptions));
        }

        [Test]
        public void Should_return_all_missing_required_options()
        {
            // missing required int option and string option
            parser = new CommandLineParser("/BoolOption");
            requiredTestOptions = parser.BuildOptions(requiredTestOptions);

            IList<Option> missingOptions = parser.FindMissingRequiredOptions(requiredTestOptions);
            Assert.AreEqual(2, missingOptions.Count);
            Assert.IsNotNull(missingOptions[0].LongName);
        }

        [Test]
        public void Can_handle_params_within_params()
        {
            parser = new CommandLineParser(@"/StringOption=/p:msbuildprop1=prop1val");
            testOptions = parser.BuildOptions(testOptions);
            Assert.AreEqual("/p:msbuildprop1=prop1val", testOptions.StringOption);
        }

        [Test, Ignore]
        public void Can_handle_multiple_params_within_params()
        {
            parser = new CommandLineParser(@"""/StringOption=/p:msbuildprop1=prop1val /p:msbuildprop2=prop2val""");
            testOptions = parser.BuildOptions(testOptions);
            Assert.AreEqual("/p:msbuildprop1=prop1val /p:msbuildprop2=prop2val", testOptions.StringOption);
        }

        [Test, Ignore]
        public void Parsing_command_line_removes_exe()
        {
            parser = new CommandLineParser(@"d:\source\my special folder\some.exe /p:prop=val");
            Assert.AreEqual("/p:prop=val", parser.CommandLine);
        }

        [Test]
        public void Should_turn_string_collection_into_generic_list_of_string()
        {
            parser = new CommandLineParser(@"/StringList=val1;val2;val3");
            testOptions = parser.BuildOptions(testOptions);

            Assert.AreEqual(3, testOptions.StringList.Count);
            Assert.AreEqual("val1", testOptions.StringList[0]);
            Assert.AreEqual("val2", testOptions.StringList[1]);
            Assert.AreEqual("val3", testOptions.StringList[2]);
        }
    }

    public class TestOptions
    {
        private bool boolOption;
        private int intOption;
        private string stringOption;
        private List<string> _stringList;

        [Option(
            LongName = "StringOption",
            ShortName = "s",
            Description = "Some sort of string option")]
        public string StringOption
        {
            get { return stringOption; }
            set { stringOption = value; }
        }

        [Option(
            LongName = "BoolOption",
            ShortName = "b",
            Description = "Some sort of bool option")]
        public bool BoolOption
        {
            get { return boolOption; }
            set { boolOption = value; }
        }

        [Option(
            LongName = "IntOption",
            ShortName = "i",
            Description = "Some sort of int option")]
        public int IntOption
        {
            get { return intOption; }
            set { intOption = value; }
        }

        [Option(
            LongName = "StringList",
            Description = "Some sort of string collection option")]
        public IList<string> StringList
        {
            get { return _stringList.AsReadOnly(); }
            set { _stringList = new List<string>(value); }
        }
    }

    public class RequiredTestOptions
    {
        private bool boolOption;
        private int intOption;
        private string stringOption;

        [Option(
            LongName = "StringOption",
            Description = "Some sort of string option",
            Required = true)]
        public string StringOption
        {
            get { return stringOption; }
            set { stringOption = value; }
        }

        [Option(
            LongName = "BoolOption",
            Description = "Some sort of bool option",
            Required = true)]
        public bool BoolOption
        {
            get { return boolOption; }
            set { boolOption = value; }
        }

        [Option(
            LongName = "IntOption",
            Description = "Some sort of int option",
            Required = true)]
        public int IntOption
        {
            get { return intOption; }
            set { intOption = value; }
        }        
    }
}