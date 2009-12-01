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
using System.Collections.Generic;
using NUnit.Framework;

namespace Sneal.CmdLineParser.Tests
{
    [TestFixture]
    public class CommandLineParserTests
    {
        private TestOptions _testOptions;
        private CommandLineParser<TestOptions> _parser;
        private CommandLineParser<RequiredTestOptions> _requiredParser;

        [Test]
        public void Options_should_be_empty_list_before_BuildOptions_is_called()
        {
            _parser = new CommandLineParser<TestOptions>();
            Assert.IsNotNull(_parser.Options);
            Assert.AreEqual(0, _parser.Options.Count);
        }

        [Test]
        public void ShouldGetSettableProperties()
        {
            _parser = new CommandLineParser<TestOptions>();
            _parser.BuildOptions();
            var settableOptions = _parser.Options;

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
            CreateOptionsParser("/StringOption=Sneal /IntOption=22");
            _testOptions = _parser.BuildOptions(_testOptions);
            Assert.IsFalse(_testOptions.BoolOption);
        }

        [Test]
        public void Should_set_options_using_long_names()
        {
            CreateOptionsParser("/StringOption=Sneal /BoolOption /IntOption=22");

            _testOptions = _parser.BuildOptions(_testOptions);

            Assert.AreEqual("Sneal", _testOptions.StringOption);
            Assert.IsTrue(_testOptions.BoolOption);
            Assert.AreEqual(22, _testOptions.IntOption);
        }

        [Test]
        public void Should_set_options_using_short_names()
        {
            CreateOptionsParser("/s=Sneal /b /i=22");

            _testOptions = _parser.BuildOptions(_testOptions);

            Assert.AreEqual("Sneal", _testOptions.StringOption);
            Assert.IsTrue(_testOptions.BoolOption);
            Assert.AreEqual(22, _testOptions.IntOption);
        }

        [Test]
        public void Should_ignore_extra_spaces()
        {
            CreateOptionsParser("  /StringOption=Sneal  /BoolOption  /IntOption=22 ");

            _testOptions = _parser.BuildOptions(_testOptions);

            Assert.AreEqual("Sneal", _testOptions.StringOption);
            Assert.IsTrue(_testOptions.BoolOption);
            Assert.AreEqual(22, _testOptions.IntOption);
        }

        [Test]
        public void Should_work_with_dashes()
        {
            _parser = new CommandLineParser<TestOptions>("-StringOption=Sneal");
            _testOptions = _parser.BuildOptions(_testOptions);
            Assert.AreEqual("Sneal", _testOptions.StringOption);
        }

        [Test]
        public void Can_explcitly_set_boolean_option()
        {
            CreateOptionsParser("/BoolOption=true");
            _testOptions = _parser.BuildOptions(_testOptions);
            Assert.IsTrue(_testOptions.BoolOption);
        }

        [Test]
        public void Can_implcitly_set_boolean_option()
        {
            CreateOptionsParser("/BoolOption");
            _testOptions = _parser.BuildOptions(_testOptions);
            Assert.IsTrue(_testOptions.BoolOption);
        }

        [Test]
        public void Should_not_require_value_for_string()
        {
            CreateOptionsParser("/StringOption");
            _parser.BuildOptions(_testOptions);
        }

        [Test]
        public void Should_not_require_value_for_int()
        {
            CreateOptionsParser("/IntOption");
            _parser.BuildOptions(_testOptions);
        }

        [Test]
        public void Can_new_up_parser_with_empty_ctor()
        {
            new CommandLineParser<TestOptions>();
        }

        [Test]
        public void Should_return_true_if_a_required_param_is_missing()
        {
            // missing required int option
            CreateRequiredOptionsParser("/StringOption=Sneal /BoolOption");
            _requiredParser.BuildOptions();
            Assert.IsTrue(_requiredParser.IsMissingRequiredOptions());
        }

        [Test]
        public void Should_return_false_if_an_optional_param_is_missing()
        {
            // missing optional int option
            CreateOptionsParser("/StringOption=Sneal /BoolOption");
            _testOptions = _parser.BuildOptions();
            Assert.IsFalse(_parser.IsMissingRequiredOptions());
        }

        [Test]
        public void Should_return_all_missing_required_options()
        {
            // missing required int option and string option
            CreateRequiredOptionsParser("/BoolOption");
            _requiredParser.BuildOptions();

            IList<Option> missingOptions = _requiredParser.FindMissingRequiredOptions();
            Assert.AreEqual(2, missingOptions.Count);
            Assert.IsNotNull(missingOptions[0].LongName);
        }

        [Test]
        public void Can_handle_params_within_params()
        {
            CreateOptionsParser(@"/StringOption=/p:msbuildprop1=prop1val");
            _testOptions = _parser.BuildOptions(_testOptions);
            Assert.AreEqual("/p:msbuildprop1=prop1val", _testOptions.StringOption);
        }

        [Test, Ignore]
        public void Can_handle_multiple_params_within_params()
        {
            CreateOptionsParser(@"""/StringOption=/p:msbuildprop1=prop1val /p:msbuildprop2=prop2val""");
            _testOptions = _parser.BuildOptions(_testOptions);
            Assert.AreEqual("/p:msbuildprop1=prop1val /p:msbuildprop2=prop2val", _testOptions.StringOption);
        }

        [Test, Ignore]
        public void Parsing_command_line_removes_exe()
        {
            CreateOptionsParser(@"d:\source\my special folder\some.exe /p:prop=val");
            Assert.AreEqual("/p:prop=val", _parser.CommandLine);
        }

        [Test]
        public void Should_turn_string_collection_into_generic_list_of_string()
        {
            CreateOptionsParser(@"/StringList=val1;val2;val3");
            _testOptions = _parser.BuildOptions(_testOptions);

            Assert.AreEqual(3, _testOptions.StringList.Count);
            Assert.AreEqual("val1", _testOptions.StringList[0]);
            Assert.AreEqual("val2", _testOptions.StringList[1]);
            Assert.AreEqual("val3", _testOptions.StringList[2]);
        }

        [Test]
        public void Should_expand_environment_variables()
        {
            CreateOptionsParser(@"/StringOption=%computername%");
            _testOptions = _parser.BuildOptions(_testOptions);

            Assert.AreEqual(Environment.MachineName, _testOptions.StringOption);
        }

        [Test]
        public void Should_not_expand_environment_variables_when_ExpandEnvironmentVariables_is_false()
        {
            CreateOptionsParser(@"/StringOption=%computername%");
            _parser.ExpandEnvironmentVariables = false;
            _testOptions = _parser.BuildOptions(_testOptions);

            Assert.AreEqual("%computername%", _testOptions.StringOption);
        }

        [Test]
        public void Should_create_new_TestOptions_instance()
        {
            CreateOptionsParser(@"");
            _testOptions = _parser.BuildOptions();
            Assert.IsNotNull(_testOptions);
        }

        private void CreateOptionsParser(string args)
        {
            _parser = new CommandLineParser<TestOptions>(args);
            _testOptions = new TestOptions();
        }

        private void CreateRequiredOptionsParser(string args)
        {
            _requiredParser = new CommandLineParser<RequiredTestOptions>(args);
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