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

using System;
using System.Linq;
using System.Collections.Generic;
using NUnit.Framework;

namespace Sneal.CmdLineParser.Tests
{
	[TestFixture]
    public class CommandLineParserTests
    {
        private CommandLineParser _parser;

        [Test]
        public void GetOptions_should_be_empty_list_before_BuildOptions_is_called()
        {
            var parser = new CommandLineParser();
            Assert.IsNotNull(parser.GetOptions());
            Assert.AreEqual(0, parser.GetOptions().Count());
        }

        [Test]
        public void ShouldGetSettableProperties()
        {
            CreateTestOptions("");
			var settableOptions = new List<Option>(_parser.GetOptions());

            Assert.IsNotNull(settableOptions);
            Assert.That(settableOptions.Count == 5);
        }

        [Test]
        public void Boolean_option_should_be_false_when_not_specified()
        {
            var options = CreateTestOptions("/StringOption=Sneal /IntOption=22");
            Assert.IsFalse(options.BoolOption);
        }

        [Test]
        public void Should_set_options_using_long_names()
        {
            var options = CreateTestOptions("/StringOption=Sneal /BoolOption /IntOption=22");
            Assert.AreEqual("Sneal", options.StringOption);
            Assert.IsTrue(options.BoolOption);
            Assert.AreEqual(22, options.IntOption);
        }

        [Test]
        public void Should_set_options_using_short_names()
        {
            var options = CreateTestOptions("/s=Sneal /b /i=22");
            Assert.AreEqual("Sneal", options.StringOption);
            Assert.IsTrue(options.BoolOption);
            Assert.AreEqual(22, options.IntOption);
        }

        [Test]
        public void Should_ignore_extra_spaces()
        {
            var options = CreateTestOptions("  /StringOption=Sneal  /BoolOption  /IntOption=22 ");
            Assert.AreEqual("Sneal", options.StringOption);
            Assert.IsTrue(options.BoolOption);
            Assert.AreEqual(22, options.IntOption);
        }

        [Test]
        public void Should_work_with_dashes()
        {
            var options = CreateTestOptions("-StringOption=Sneal");
            Assert.AreEqual("Sneal", options.StringOption);
        }

        [Test]
        public void Can_explicitly_set_boolean_option()
        {
            var options = CreateTestOptions("/BoolOption=true");
            Assert.IsTrue(options.BoolOption);
        }

        [Test]
        public void Can_implicitly_set_boolean_option()
        {
            var options = CreateTestOptions("/BoolOption");
            Assert.IsTrue(options.BoolOption);
        }

        [Test]
        public void Should_not_require_value_for_string()
        {
            var options = CreateTestOptions("/StringOption");
            _parser.BuildOptions(options);
        }

        [Test]
        public void Should_not_require_value_for_int()
        {
            var options = CreateTestOptions("/IntOption");
            _parser.BuildOptions(options);
        }

        [Test]
        public void Can_new_up_parser_with_empty_ctor()
        {
            new CommandLineParser();
        }

        [Test]
        public void Should_return_true_if_a_required_param_is_missing()
        {
            // missing required int option
            CreateRequiredTestOptions("/StringOption=Sneal /BoolOption");
            Assert.IsTrue(_parser.IsMissingRequiredOptions());
        }

        [Test]
        public void Should_return_false_if_an_optional_param_is_missing()
        {
            // missing optional int option
            CreateTestOptions("/StringOption=Sneal /BoolOption");
            Assert.IsFalse(_parser.IsMissingRequiredOptions());
        }

        [Test]
        public void Should_return_all_missing_required_options()
        {
            // missing required int option and string option
            CreateRequiredTestOptions("/BoolOption");
            var missingOptions = new List<Option>(_parser.GetMissingRequiredOptions());
            Assert.AreEqual(2, missingOptions.Count);
            Assert.IsNotNull(missingOptions[0].LongName);
        }

        [Test]
        public void Can_handle_params_within_params()
        {
            var options = CreateTestOptions(@"/StringOption=/p:msbuildprop1=prop1val");
            Assert.AreEqual("/p:msbuildprop1=prop1val", options.StringOption);
        }

        [Test, Ignore]
        public void Can_handle_multiple_params_within_params()
        {
            var options = CreateTestOptions(@"""/StringOption=/p:msbuildprop1=prop1val /p:msbuildprop2=prop2val""");
            Assert.AreEqual("/p:msbuildprop1=prop1val /p:msbuildprop2=prop2val", options.StringOption);
        }

        [Test]
        public void Should_turn_string_collection_into_generic_list_of_string()
        {
            var options = CreateTestOptions(@"/StringList=val1;val2;val3");
            Assert.AreEqual(3, options.StringList.Count);
            Assert.AreEqual("val1", options.StringList[0]);
            Assert.AreEqual("val2", options.StringList[1]);
            Assert.AreEqual("val3", options.StringList[2]);
        }

        [Test]
        public void Should_create_new_TestOptions_instance()
        {
            var options1 = CreateTestOptions("");
			var options2 = CreateTestOptions("");
            Assert.AreNotSame(options1, options2);
        }

		[Test]
		public void Can_create_options_with_enum()
		{
			var options = CreateTestOptions(@"/mode=Mode2");
			Assert.AreEqual(TestOptions.OperationMode.Mode2, options.Mode);
		}

		[Test]
		public void Bad_enum_value_throws_CommandLineException()
		{
			Assert.Throws<CommandLineException>(() => CreateTestOptions(@"/mode=ModeNonExistant"));
		}
		
		private TestOptions CreateTestOptions(string args)
		{
			_parser = new CommandLineParser(args);
			return _parser.BuildOptions<TestOptions>();
		}
		
		private RequiredTestOptions CreateRequiredTestOptions(string args)
		{
			_parser = new CommandLineParser(args);
			return _parser.BuildOptions<RequiredTestOptions>();
		}		
    }

    public class TestOptions
    {
		public enum OperationMode
		{
			Mode1,
			Mode2
		}

        private bool boolOption;
        private int intOption;
        private string stringOption;
        private List<string> _stringList;

		[Option(LongName="mode", ShortName = "m")]
		public OperationMode Mode { get; set; }

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