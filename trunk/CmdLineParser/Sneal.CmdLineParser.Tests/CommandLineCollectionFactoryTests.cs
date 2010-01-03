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
using NUnit.Framework;
using Sneal.CmdLineParser;

namespace Sneal.CmdLineParser.Tests
{
	[TestFixture]
	public class CommandLineCollectionFactoryTests
	{
		private CommandLineCollectionFactory _factory;
		
		[SetUp]
		public void SetUp()
		{
			_factory = new CommandLineCollectionFactory();
		}
		
		[Test]
		public void CreateCommandLineCollection_creates_a_collection_when_passed_null()
		{
			Assert.IsNotNull(_factory.CreateCommandLineCollection(null));
		}
		
		[Test]
		public void CreateCommandLineCollection_creates_a_collection_when_passed_empty_string()
		{
			Assert.IsNotNull(_factory.CreateCommandLineCollection(""));
		}
		
		[Test]
		public void ExpandEnvironmentVariables_defaults_to_true()
		{
			Assert.IsTrue(_factory.ExpandEnvironmentVariables);
		}			
		
		[Test]
		public void CreateCommandLineCollection_splits_on_space_dash()
		{
			var options = _factory.CreateCommandLineCollection(" -m -n");
			Assert.AreEqual(2, options.Count);
			Assert.IsTrue(options.Contains("m"));
			Assert.IsTrue(options.Contains("n"));
		}		
		
		[Test]
		public void CreateCommandLineCollection_splits_on_space_slash()
		{
			var options = _factory.CreateCommandLineCollection(" /m /n");
			Assert.AreEqual(2, options.Count);
			Assert.IsTrue(options.Contains("m"));
			Assert.IsTrue(options.Contains("n"));
		}	
		
		[Test]
		public void CreateCommandLineCollection_ignores_extra_whitespace()
		{
			var options = _factory.CreateCommandLineCollection("/m   /n");
			Assert.AreEqual(2, options.Count);
			Assert.IsTrue(options.Contains("m"));
			Assert.IsTrue(options.Contains("n"));
		}	
		
		[Test]
		public void CreateCommandLineCollection_expands_environment_variables()
		{
			Environment.SetEnvironmentVariable("PCNAME", "SERVER1");
			var options = _factory.CreateCommandLineCollection("/m=%PCNAME%");
			Assert.AreEqual("SERVER1", options.GetValue("m"));
		}	
		
		[Test]
		public void CreateCommandLineCollection_does_not_expand_environment_variables_when_off()
		{
			_factory.ExpandEnvironmentVariables = false;
			Environment.SetEnvironmentVariable("PCNAME", "SERVER1");
			var options = _factory.CreateCommandLineCollection("/m=%PCNAME%");
			Assert.AreEqual("%PCNAME%", options.GetValue("m"));
		}		
	}
}
