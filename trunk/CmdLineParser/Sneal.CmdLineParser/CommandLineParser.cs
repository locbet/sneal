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
    public class CommandLineParser<T> where T : class, new()
    {
        private readonly PropertySetterRegistry _propertySetterRegistry = new PropertySetterRegistry();
        private readonly List<Option> _options = new List<Option>();

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
            CommandLineCollection = new CommandLineCollection();
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
        public T BuildOptions()
        {
            return BuildOptions(new T());
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
        public T BuildOptions(T optionsInstance)
        {
            ExpandCommandLineIntoArgs();
            CreateOptions();
            SetOptionValues(optionsInstance);
            return optionsInstance;
        }

        /// <summary>
        /// Returns true if any required options are missing their values.
        /// </summary>
        public bool IsMissingRequiredOptions()
        {
            return FindMissingRequiredOptions().Count > 0;
        }

        /// <summary>
        /// Returns a list of all the required options that have missing values.
        /// </summary>
        public IList<Option> FindMissingRequiredOptions()
        {
            var requiredOptions = _options.FindAll(o => o.IsRequired);
            var missingRequiredOptions = requiredOptions.FindAll(o => !CommandLineCollection.Contains(o));
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
        public bool ExpandEnvironmentVariables { get; set; }

        /// <summary>
        /// The raw command line with unexpanded environment variables.
        /// </summary>
        public string RawCommandLine { get; private set; }

        /// <summary>
        /// The command line with expanded environment variables.
        /// </summary>
        public string CommandLine { get; private set; }

        /// <summary>
        /// Collection of split raw string options from the command line, keyed
        /// by argument name.
        /// </summary>
        public CommandLineCollection CommandLineCollection { get; private set; }

        /// <summary>
        /// List of finished options created from the options instance.
        /// </summary>
        /// <remarks>
        /// The option instances are direct references and can be modified,
        /// for instance if you optionally have required options.
        /// </remarks>
        public IList<Option> Options
        {
            get { return _options; }
        }

        private void CreateOptions()
        {
            _options.Clear();
            PropertyInfo[] props = typeof(T).GetProperties();
            foreach (PropertyInfo prop in props)
            {
                object[] attrs = prop.GetCustomAttributes(typeof(OptionAttribute), true);
                if (attrs == null || attrs.Length == 0)
                    continue;

                IPropertySetter propertySetter = _propertySetterRegistry.GetPropertySetter(prop.PropertyType);
                var swAttr = (OptionAttribute)attrs[0];
                _options.Add(Option.Create(swAttr, prop, propertySetter));
            }
        }

        private void SetOptionValues(T optionsInstance)
        {
            foreach (Option option in _options)
            {
                string optionValue = CommandLineCollection.GetValue(option);
                if (CommandLineCollection.Contains(option))
                {
                    option.SetValue(optionsInstance, optionValue);
                }
            }
        }

        private void ExpandCommandLineIntoArgs()
        {
            CommandLine = (RawCommandLine ?? "").Trim();
            if (ExpandEnvironmentVariables)
            {
                CommandLine = Environment.ExpandEnvironmentVariables(CommandLine);
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
            foreach (string rawOption in splitReg.Split(CommandLine))
            {
                CommandLineCollection.Add(rawOption);
            }
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