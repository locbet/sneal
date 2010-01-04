#region license
// Copyright 2010 Shawn Neal (neal.shawn@gmail.com)
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
using Sneal.CmdLineParser.PropertySetters;

namespace Sneal.CmdLineParser.Tests
{
	[TestFixture]
	public class ReflectiveOptionsBuilderTests
	{
		private ReflectiveOptionsBuilder _builder;
		
		[SetUp]
		public void SetUp()
		{
			_builder = new ReflectiveOptionsBuilder(new PropertySetterRegistry());	
		}
		
		[Test]
		public void BuildOptions_should_build_3_options()
		{
			IList<Option> options = _builder.BuildOptions<ReflectiveTestOptions>();
			Assert.AreEqual(3, options.Count);
		}
		
		[Test]
		public void BuildOptions_should_create_string_option_correctly()
		{
			List<Option> options = new List<Option>(_builder.BuildOptions<ReflectiveTestOptions>());
			Option stringOption = options.Find(o => o.ShortName == "s");
			Assert.IsNotNull(stringOption);
			Assert.AreEqual("StringOption", stringOption.LongName);
			Assert.AreEqual("Some sort of string option", stringOption.Description);
			Assert.AreEqual("s", stringOption.ShortName);
		}		
	}
				
    public class ReflectiveTestOptions
    {
        [Option(
            LongName = "StringOption",
            ShortName = "s",
            Description = "Some sort of string option")]
        public string StringOption { get; set; }

        [Option(
            LongName = "BoolOption",
            ShortName = "b",
            Description = "Some sort of bool option")]
        public bool BoolOption { get; set; }

        [Option(
            LongName = "IntOption",
            ShortName = "i",
            Description = "Some sort of int option",
		    Required = true)]
        public int IntOption { get; set; }
		
		public string IgnoredProperty { get; set;}
    }
}
