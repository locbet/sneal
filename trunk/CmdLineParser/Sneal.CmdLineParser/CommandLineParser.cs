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
using System.Reflection;
using System.Text.RegularExpressions;
using Sneal.CmdLineParser.PropertySetters;

namespace Sneal.CmdLineParser
{
    public class CommandLineParser
    {
        public static readonly char[] SwitchChars = {'-', '/'};

        private readonly PropertySetterRegistry _propertySetterRegistry = new PropertySetterRegistry();
        private readonly CommandLineCollection _cmdLineCollection = new CommandLineCollection();
        private IUsageFormatter _usageFormatter = new DefaultUsageFormatter();

        private string _commandLine;
        private bool _expandEnvironmentVariables = true;
        private readonly string _rawCommandLine;

        /// <summary>
        /// Constructs a new command line parser instance from Environment.GetCommandLineArgs()
        /// </summary>
        public CommandLineParser()
            : this(Environment.GetCommandLineArgs()) { }

        /// <summary>
        /// Constructs a new command line parser instance from using the specified
        /// command line args.  Generally call this with the args from Main().
        /// </summary>
        public CommandLineParser(string[] commandLineArgs)
            : this(string.Join(" ", commandLineArgs)) { }

        /// <summary>
        /// Constructs a new command line parser instance using the specified
        /// command line string.
        /// </summary>
        /// <param name="commandLineArgs">
        /// The command arguments without the executable name and path.
        /// </param>
        public CommandLineParser(string commandLineArgs)
        {
            _rawCommandLine = commandLineArgs;
            RegisterDefaultPropertySetters();
        }

        /// <summary>
        /// Registers a property strategy with the command line parser.
        /// </summary>
        /// <param name="propertySetter">The instance to register</param>
        public void RegisterPropertySetter(IPropertySetter propertySetter)
        {
            _propertySetterRegistry.RegisterPropertySetter(propertySetter);
        }

        /// <summary>
        /// Sets any properties on the given object that have a OptionAttribute
        /// that matches the command line args.
        /// <para>Valid switch key/value separators: '=' ':'</para>
        /// <para>Valid flag delimters: '/' '-'</para>
        /// <para>Example command lines arguments:</para>
        /// <list>
        /// <item>/help</item>
        /// <item>-help</item>
        /// <item>-h</item>
        /// <item>/server=localhost</item>
        /// <item>/server:localhost /db:Northwind</item>
        /// <item>-server=localhost -db=Northwind</item>
        /// </list>
        /// </summary>
        /// <param name="optionsInstance">The instance to reflect upon and set values.</param>
        public T BuildOptions<T>(T optionsInstance) where T : class
        {
            ExpandCommandLineIntoArgs();
            List<Option> options = GetSettableOptions(optionsInstance);
            foreach (Option option in options)
            {
                string optionValue = _cmdLineCollection.GetValue(option);
                if (_cmdLineCollection.Contains(option))
                {
                    option.SetValue(optionsInstance, optionValue);
                }
            }
            return optionsInstance;
        }

        /// <summary>
        /// Returns true if any required options are missing their values.
        /// </summary>
        public bool IsMissingRequiredOptions<T>(T optionsInstance) where T : class
        {
            return FindMissingRequiredOptions(optionsInstance).Count > 0;
        }

        /// <summary>
        /// Returns a list of all the required options that have missing values.
        /// </summary>
        public IList<Option> FindMissingRequiredOptions<T>(T optionsInstance) where T : class
        {
            var requiredOptions = GetSettableOptions(optionsInstance).FindAll(o => o.IsRequired);
            var missingRequiredOptions = requiredOptions.FindAll(o => !_cmdLineCollection.Contains(o));
            return new List<Option>(missingRequiredOptions);
        }

        /// <summary>
        /// Builds a string list of all the command line flags on a given
        /// options instance.  The list contains the flag name and flag
        /// description in a 2 column formatted list.
        /// </summary>
        /// <param name="optionsInstance">
        /// The options instance to look for SwitchAttributes
        /// </param>
        /// <returns>Command line flags and descriptions.</returns>
        public string GetUsageLines(object optionsInstance)
        {
            return UsageFormatter.GetUsage(GetSettableOptions(optionsInstance));
        }

        /// <summary>
        /// Gets a list of settable option instances.
        /// </summary>
        /// <param name="optionsInstance">The options instance to look for SwitchAttributes.</param>
        public List<Option> GetSettableOptions(object optionsInstance)
        {
            var options = new List<Option>();
            if (optionsInstance == null)
            {
                return options;
            }

            PropertyInfo[] props = optionsInstance.GetType().GetProperties();
            foreach (PropertyInfo prop in props)
            {
                object[] attrs = prop.GetCustomAttributes(typeof(OptionAttribute), true);
                if (attrs == null || attrs.Length == 0)
                    continue;

                IPropertySetter propertySetter = _propertySetterRegistry.GetPropertySetter(prop.PropertyType);
                var swAttr = (OptionAttribute)attrs[0];
                options.Add(Option.Create(swAttr, prop, propertySetter));
            }

            return options;
        }

        /// <summary>
        /// Gets/Sets the help screen usage formatter instance.
        /// </summary>
        public IUsageFormatter UsageFormatter
        {
            get { return _usageFormatter; }
            set { _usageFormatter = value; }
        }

        /// <summary>
        /// Whether or not to expand any environement variables found in the
        /// command line.  The default is <c>true</c>.
        /// </summary>
        public bool ExpandEnvironmentVariables
        {
            get { return _expandEnvironmentVariables; }
            set { _expandEnvironmentVariables = value; }
        }

        /// <summary>
        /// The raw command line with unexpanded environment variables.
        /// </summary>
        public string RawCommandLine
        {
            get { return _rawCommandLine; }
        }

        /// <summary>
        /// The command line with expanded environment variables.
        /// </summary>
        public string CommandLine
        {
            get { return _commandLine; }
        }

        /// <summary>
        /// Collection of parsed Option instances.
        /// </summary>
        public CommandLineCollection CommandLineCollection
        {
            get { return _cmdLineCollection; }
        }

        private void ExpandCommandLineIntoArgs()
        {
            _commandLine = (_rawCommandLine ?? "").Trim();
            if (ExpandEnvironmentVariables)
            {
                _commandLine = Environment.ExpandEnvironmentVariables(_commandLine);
            }
            SplitCommandLineIntoArgs();
        }

        /// <summary>
        /// Splits a full command line in a way that supports lists
        /// </summary>
        private void SplitCommandLineIntoArgs()
        {
            // TODO: take into account "" around args for args within args?
            // TODO: Make this into a strategy?  Like GNU option parser strategy etc?
            var splitReg = new Regex(" [/|-]");
            foreach (string rawOption in splitReg.Split(_commandLine))
            {
                _cmdLineCollection.Add(rawOption);
            }
        }

        private void RegisterDefaultPropertySetters()
        {
            RegisterPropertySetter(new BooleanPropertySetter());
            RegisterPropertySetter(new StringPropertySetter());
            RegisterPropertySetter(new IntegerPropertySetter());
            RegisterPropertySetter(new StringListPropertySetter());
        }

    }
}