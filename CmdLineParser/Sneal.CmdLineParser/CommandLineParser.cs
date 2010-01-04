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
using System.Linq;
using Sneal.CmdLineParser.PropertySetters;

namespace Sneal.CmdLineParser
{
    /// <summary>
    /// Main class for parsing arguments.
    /// <example>
    /// var parser = new CommandLineParser(args);
    /// MyOptions options = parser.BuildOptions(new MyOptions());
    /// </example>
    /// </summary>
    /// <remarks>
    /// The parser holds state and cannot be reused between different option
    /// instances any longer.
    /// </remarks>
    public class CommandLineParser
    {
		private CommandLineCollection _commandLineCollection = new CommandLineCollection();
		private List<Option> _options = new List<Option>();
		private readonly CommandLineCollectionFactory _commandLineCollectionFactory = new CommandLineCollectionFactory();
        private readonly PropertySetterRegistry _propertySetterRegistry = new PropertySetterRegistry();

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
            UsageFormatter = new DefaultUsageFormatter();
            ExpandEnvironmentVariables = true;
            RawCommandLine = commandLineArgs;
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
        /// that matches the command line args. This creates a new options
        /// instance of type T.
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
        public TOption BuildOptions<TOption>() where TOption : class, new()
        {
            return (TOption) BuildOptions(new TOption());
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
        public TOption BuildOptions<TOption>(TOption optionsInstance)
        {
            CreateCommandLineCollection();
            CreateOptions(typeof(TOption));
            SetOptionValues(optionsInstance);
            return optionsInstance;
        }
		
        /// <summary>
        /// Returns the last set of built options that have missing required values.
        /// </summary>
        public IEnumerable<Option> GetMissingRequiredOptions()
        {
            var requiredOptions = _options.FindAll(o => o.IsRequired);
            return requiredOptions.FindAll(o => !_commandLineCollection.Contains(o));
        }
		
        /// <summary>
        /// Returns the last set of built options.
        /// </summary>		
		public IEnumerable<Option> GetOptions()
		{
			return _options;
		}

        /// <summary>
        /// Returns true if any required options are missing their values.
        /// </summary>
        public bool IsMissingRequiredOptions()
        {
            return GetMissingRequiredOptions().Count() > 0;
        }

        /// <summary>
        /// Builds a string list of all the command line flags on a given
        /// options instance.  The list contains the flag name and flag
        /// description in a 2 column formatted list.
        /// </summary>
        /// <returns>Command line flags and descriptions.</returns>
        public string GetUsageLines()
        {
            return UsageFormatter.GetUsage(_options);
        }

        /// <summary>
        /// Gets/Sets the help screen usage formatter instance.
        /// </summary>
        public IUsageFormatter UsageFormatter { get; set; }

        /// <summary>
        /// Whether or not to expand any environement variables found in the
        /// command line.  The default is <c>true</c>.
        /// </summary>
        public bool ExpandEnvironmentVariables
		{
			get { return _commandLineCollectionFactory.ExpandEnvironmentVariables; }
			set { _commandLineCollectionFactory.ExpandEnvironmentVariables = value; }
		}

        /// <summary>
        /// The raw command line with unexpanded environment variables.
        /// </summary>
        public string RawCommandLine { get; private set; }

        private void CreateOptions(Type optionsType)
        {
            var optionsBuilder = new ReflectiveOptionsBuilder(_propertySetterRegistry);
			_options = new List<Option>(optionsBuilder.BuildOptions(optionsType));
        }

        private void SetOptionValues(object optionsInstance)
        {
            foreach (Option option in _options)
            {
                if (_commandLineCollection.Contains(option))
                {
					string optionValue = _commandLineCollection.GetValue(option);
                    option.SetValue(optionsInstance, optionValue);
                }
            }
        }

        private void CreateCommandLineCollection()
        {
            _commandLineCollection = _commandLineCollectionFactory.CreateCommandLineCollection(RawCommandLine);
        }

        private void RegisterDefaultPropertySetters()
        {
            RegisterPropertySetter(new BooleanPropertySetter());
            RegisterPropertySetter(new StringPropertySetter());
            RegisterPropertySetter(new IntegerPropertySetter());
            RegisterPropertySetter(new StringListPropertySetter());
            RegisterPropertySetter(new EnumPropertySetter());
        }
    }
}