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
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Reflection;
using System.Text.RegularExpressions;
using Sneal.CmdLineParser.PropertySetters;

namespace Sneal.CmdLineParser
{
    public class CommandLineParser
    {
        public static readonly char[] SwitchChars = {'-', '/'};

        private readonly Dictionary<Type, IPropertySetter> _setterStrategies = new Dictionary<Type, IPropertySetter>();
        private List<string> _rawArgs;
        private string _commandLine;

        /// <summary>
        /// Constructs a new command line parser instance.
        /// </summary>
        /// <param name="rawArgs">The raw command line arguments, usually from main</param>
        public CommandLineParser(IEnumerable<string> rawArgs)
        {
            SetCommandLine(rawArgs);
            RegisterDefaultPropertySetters();
        }

        private void SetCommandLine(IEnumerable<string> rawArgs)
        {
            _commandLine = "";
            foreach (var arg in rawArgs)
            {
                _commandLine += arg + " ";
            }
            _commandLine = _commandLine.Trim();
        }

        protected void RegisterDefaultPropertySetters()
        {
            RegisterPropertySetter(new BooleanPropertySetter());
            RegisterPropertySetter(new StringPropertySetter());
            RegisterPropertySetter(new IntegerPropertySetter());
            RegisterPropertySetter(new StringListPropertySetter());
        }

        public void RegisterPropertySetter(IPropertySetter setter)
        {
            if (setter == null)
                throw new ArgumentNullException("setter");

            _setterStrategies[setter.SupportedType] = setter;
        }

        /// <summary>
        /// Sets any properties on the given object that have a SwitchAttribute
        /// that matches the raw args list.
        /// <para>Valid switch key/value separators: '=' ':'</para>
        /// <para>Valid flag delimters: '/' '-'</para>
        /// <para>Example command lines arguments:</para>
        /// <list>
        /// <item>/help</item>
        /// <item>-help</item>
        /// <item>/server=localhost</item>
        /// <item>/server:localhost /db:Northwind</item>
        /// <item>-server=localhost -db=Northwind</item>
        /// </list>
        /// </summary>
        /// <param name="optionsInstance">The instance to reflect upon and set values.</param>
        public T BuildOptions<T>(T optionsInstance) where T : class
        {
            if (optionsInstance == null)
                throw new ArgumentNullException("optionsInstance");

            SplitCommandLineIntoArgs();

            List<PropertyInfoSwitchAttributePair> options = GetSettableOptions(optionsInstance);

            foreach (PropertyInfoSwitchAttributePair flag in options)
            {
                string usedArg = FindRawArgBySwitchName(flag.SwitchAttribute.Name);
                if (!string.IsNullOrEmpty(usedArg))
                {
                    SetArgValue(usedArg, optionsInstance, flag);
                }
            }

            return optionsInstance;
        }

        /// <summary>
        /// Splits a full command line in a way that supports lists
        /// </summary>
        private void SplitCommandLineIntoArgs()
        {
            // TODO: take into account "" around args for args within args
            Regex splitReg = new Regex(" [/|-]");
            _rawArgs = new List<string>();
            foreach (string arg in splitReg.Split(_commandLine))
            {
                _rawArgs.Add(StripSwitchChar(arg));
            }
        }

        /// <summary>
        /// Checks all options to ensure that any required options have been set.
        /// </summary>
        public void EnsureRequiredOptions<T>(T optionsInstance) where T : class
        {
            if (optionsInstance == null)
                throw new ArgumentNullException("optionsInstance");

            List<PropertyInfoSwitchAttributePair> options = GetSettableOptions(optionsInstance);

            foreach (PropertyInfoSwitchAttributePair switchAttributePair in options)
            {
                if (switchAttributePair.SwitchAttribute.Required)
                {
                    string usedArg = FindRawArgBySwitchName(switchAttributePair.SwitchAttribute.Name);
                    if (string.IsNullOrEmpty(usedArg))
                    {
                        throw new RequiredParameterMissingException(
                            string.Format("The required parameter {0} is missing",
                            switchAttributePair.SwitchAttribute.Name));
                    }
                }
            }
        }

        /// <summary>
        /// Builds a string list of all the command line flags on a given
        /// options instance.  The list contains the flag name and flag
        /// description in a 2 column formatted list.
        /// </summary>
        /// <param name="optionsInstance">
        /// The options instance to look for SwitchAttributes
        /// </param>
        /// <returns>List of command line flags and descriptions.</returns>
        public static IList<string> GetUsageLines(object optionsInstance)
        {
            List<string> lines = new List<string>();

            if (optionsInstance == null)
                return lines;

            PropertyInfo[] props = optionsInstance.GetType().GetProperties();
            foreach (PropertyInfo prop in props)
            {
                object[] attrs = prop.GetCustomAttributes(typeof(SwitchAttribute), true);
                if (attrs == null || attrs.Length == 0)
                    continue;

                SwitchAttribute swAttr = (SwitchAttribute)attrs[0];
                string line = string.Format("{0,-20} {1}", swAttr.Name, swAttr.Description);
                lines.Add(line);
            }

            return lines;
        }

        /// <summary>
        /// Gets a dictionary of settable property meta data keyed by the flag name.
        /// </summary>
        /// <param name="optionsInstance">The options instance to look for SwitchAttributes.</param>
        /// <returns></returns>
        public virtual List<PropertyInfoSwitchAttributePair> GetSettableOptions(object optionsInstance)
        {
            List<PropertyInfoSwitchAttributePair> options = new List<PropertyInfoSwitchAttributePair>();

            if (optionsInstance == null)
                return options;

            PropertyInfo[] props = optionsInstance.GetType().GetProperties();
            foreach (PropertyInfo prop in props)
            {
                object[] attrs = prop.GetCustomAttributes(typeof(SwitchAttribute), true);
                if (attrs == null || attrs.Length == 0)
                    continue;

                SwitchAttribute swAttr = (SwitchAttribute)attrs[0];
                options.Add(new PropertyInfoSwitchAttributePair(prop, swAttr));
            }

            return options;
        }

//        /// <summary>
//        /// Gets a dictionary of settable property meta data keyed by the flag name.
//        /// </summary>
//        /// <param name="optionsInstance">The options instance to look for SwitchAttributes.</param>
//        /// <returns></returns>
//        public virtual Dictionary<string, PropertyInfoSwitchAttributePair> GetSettableOptions(object optionsInstance)
//        {
//            Dictionary<string, PropertyInfoSwitchAttributePair> options = new Dictionary<string, PropertyInfoSwitchAttributePair>();
//
//            if (optionsInstance == null)
//                return options;
//
//            PropertyInfo[] props = optionsInstance.GetType().GetProperties();
//            foreach (PropertyInfo prop in props)
//            {
//                object[] attrs = prop.GetCustomAttributes(typeof (SwitchAttribute), true);
//                if (attrs == null || attrs.Length == 0)
//                    continue;
//
//                SwitchAttribute swAttr = (SwitchAttribute) attrs[0];
//                PropertyInfoSwitchAttributePair pair = new PropertyInfoSwitchAttributePair(prop, swAttr);
//                options.Add(swAttr.Name, pair);
//            }
//
//            return options;
//        }

        /// <summary>
        /// List of each raw command line argument.
        /// </summary>
        public IList<string> RawArgs
        {
            get { return new ReadOnlyCollection<string>(_rawArgs); }
        }

        /// <summary>
        /// Attempts to set the associated property on the optionsInstance using
        /// the argument value. The value on the optionsInstance is set using
        /// one of the registered IPropertySetter implementations.
        /// </summary>
        /// <param name="rawArg">The raw command line arg as entered by the user.</param>
        /// <param name="optionsInstance">The options instance to set the property on.</param>
        /// <param name="propAttrPair">
        /// The meta data associated with the switch attribute and the reflection
        /// property info.
        /// </param>
        protected virtual void SetArgValue(string rawArg, object optionsInstance, PropertyInfoSwitchAttributePair propAttrPair)
        {
            if (string.IsNullOrEmpty(rawArg))
                throw new ArgumentException("rawArg cannot be null or empty");

            if (optionsInstance == null)
                throw new ArgumentNullException("optionsInstance");

            if (propAttrPair == null)
                throw new ArgumentNullException("propAttrPair");

            PropertyInfo propertyInfo = propAttrPair.PropertyInfo;
            SwitchAttribute switchAttr = propAttrPair.SwitchAttribute;

            KeyValuePair<string, string> arg = SplitFlagAndValue(rawArg, switchAttr);
            string val = arg.Value;

            if (string.IsNullOrEmpty(val))
            {
                if (propertyInfo.PropertyType == typeof(bool))
                    val = true.ToString();
                else
                {
                    throw new CmdLineParserException(string.Format(
                        "Expected a value associated with flag {0}, but no value was " +
                        "associated with the flag.",
                        switchAttr.Name));
                }
            }

            IPropertySetter setter = _setterStrategies[propertyInfo.PropertyType];
            if (setter == null)
            {
                throw new CmdLineParserException(string.Format(
                    "CmdLineParser doesn't know how to set property type {0} for flag {1}." + 
                    "Perhaps you are using an unsupported Type.  You could " + 
                    "implement a custom IPropertySetter strategy.",
                    propertyInfo.Name, switchAttr.Name));                
            }

            setter.SetPropertyValue(propAttrPair, optionsInstance, val);
        }

        /// <summary>
        /// Splits the current raw argument into a key value pair, where the
        /// key is the flag and the value is the commane line value.
        /// </summary>
        /// <param name="rawArg">The raw argument as entered by the user.</param>
        /// <param name="switchAttr">The switch.</param>
        /// <returns></returns>
        protected virtual KeyValuePair<string, string> SplitFlagAndValue(string rawArg, SwitchAttribute switchAttr)
        {
            if (string.IsNullOrEmpty(rawArg))
                throw new ArgumentException("rawArg cannot be null or empty");

            if (switchAttr == null)
                throw new ArgumentNullException("switchAttr");

            //string regex = "^[/|-](\\w+)\\s?[:|=]?\\s?(.*)";
            string regex = "^(\\w+)\\s?[:|=]?\\s?(.*)";
            RegexOptions options = ((RegexOptions.IgnorePatternWhitespace | RegexOptions.Multiline) | RegexOptions.IgnoreCase);
            Regex reg = new Regex(regex, options);

            MatchCollection matches = reg.Matches(rawArg);
            if (matches.Count == 0 || matches[0].Groups.Count == 0)
            {
                throw new CmdLineParserException(
                    string.Format(
                    "Did not find any value specified for the flag {0}",
                    switchAttr.Name));
            }

            Debug.Assert(
                string.Compare(switchAttr.Name, matches[0].Groups[1].Value, 
                    StringComparison.CurrentCultureIgnoreCase) == 0);

            return new KeyValuePair<string, string>(
                matches[0].Groups[1].Value,
                matches[0].Groups[2].Value);
        }

        /// <summary>
        /// Finds the raw argument as given by the user that matches the specified
        /// flag name.
        /// </summary>
        /// <param name="flag">The flag without the switch prefix char.</param>
        /// <returns>The raw command line argument that matches the flag.</returns>
        protected virtual string FindRawArgBySwitchName(string flag)
        {
            // BUG: This doesn't work if multiple args have the same prefix: /Arg=1 /Arg2=2
            return _rawArgs.Find(arg => arg.StartsWith(flag, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Removes any of the switch prefix characters from the raw argument.
        /// </summary>
        /// <remarks>
        /// input: /server=localhost
        /// output: server=localhost
        /// </remarks>
        /// <param name="rawArg">The raw argument.</param>
        /// <returns>The argument without the prefixed switch character.</returns>
        protected virtual string StripSwitchChar(string rawArg)
        {
            if (string.IsNullOrEmpty(rawArg))
                return "";

            foreach (char switchChar in SwitchChars)
            {
                if (rawArg[0] == switchChar)
                    return rawArg.Substring(1);
            }

            return rawArg;
        }
    }
}