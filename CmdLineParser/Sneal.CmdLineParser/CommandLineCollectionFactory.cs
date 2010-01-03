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
using System.Text.RegularExpressions;

namespace Sneal.CmdLineParser
{
	/// <summary>
	/// Splits a raw command line into a CommandLineCollection instance.
	/// </summary>
	public class CommandLineCollectionFactory
	{
		private static readonly Regex SplitRegex = new Regex(" [/|-]");
		
		public CommandLineCollectionFactory()
		{
			ExpandEnvironmentVariables = true;
		}
		
        /// <summary>
        /// Whether or not to expand any environement variables found in the
        /// command line.  The default is <c>true</c>.
        /// </summary>
        public bool ExpandEnvironmentVariables { get; set; }		
		
		/// <summary>
		/// Takes a raw command line string and splits it into a new 
		/// CommandLineCollection instance.
		/// </summary>
		/// <returns>
		/// A <see cref="CommandLineCollection"/>
		/// </returns>
		public CommandLineCollection CreateCommandLineCollection(string rawCommandLine)
		{
			var formattedCmdLine = FormatRawCommandLine(rawCommandLine);
			var commandLineCollection = new CommandLineCollection();
            foreach (string rawOption in SplitRegex.Split(formattedCmdLine))
            {
                commandLineCollection.Add(rawOption);
            }
			return commandLineCollection;
		}
		
		private string FormatRawCommandLine(string rawCommandLine)
		{
			string formattedCmdLine = (rawCommandLine ?? "").Trim();
            if (ExpandEnvironmentVariables)
            {
                formattedCmdLine = Environment.ExpandEnvironmentVariables(formattedCmdLine);
            }
			return formattedCmdLine;
		}
	}
}
