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
using System.Reflection;
using Sneal.CmdLineParser.PropertySetters;

namespace Sneal.CmdLineParser
{
    public class CommandLineParser
    {
        private List<string> rawArgs;
        private readonly Dictionary<Type, IPropertySetter> setterStrategies = new Dictionary<Type, IPropertySetter>();

        /// <summary>
        /// Constructs a new command line parser instance.
        /// </summary>
        /// <param name="rawArgs">The raw command line arguments, usually from main</param>
        public CommandLineParser(IEnumerable<string> rawArgs)
        {
            this.rawArgs = new List<string>(rawArgs);

            // register default types
            RegisterPropertySetter(new BooleanPropertySetter());
            RegisterPropertySetter(new StringPropertySetter());
            RegisterPropertySetter(new IntegerPropertySetter());
        }

        public void RegisterPropertySetter(IPropertySetter setter)
        {
            if (setter == null)
                throw new ArgumentNullException("setter");

            setterStrategies[setter.SupportedType] = setter;
        }

        /// <summary>
        /// Valid switch key/value separators: '=' ':'
        /// Valid flag delimters: '/' '-'
        /// <example>
        /// /help
        /// -help
        /// /server:localhost /db:Northwind
        /// -server=localhost -db=Northwind
        /// </example>
        /// </summary>
        /// <param name="optionsInstance"></param>
        public T BuildOptions<T>(T optionsInstance)
        {
            if (optionsInstance == null)
                throw new ArgumentNullException("optionsInstance");

            Dictionary<string, PropertyInfoSwitchAttributePair> options = GetSettableOptions(optionsInstance);

            foreach (string flag in options.Keys)
            {
                string usedArg = FindCommandLineArg(flag);
                if (string.IsNullOrEmpty(usedArg))
                    continue;

                SetArgValue(usedArg, optionsInstance, options[flag]);
            }

            return optionsInstance;
        }

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
                string line = string.Format("/{0,-20} {1}", swAttr.Name, swAttr.Description);
                lines.Add(line);
            }

            return lines;
        }

        public virtual Dictionary<string, PropertyInfoSwitchAttributePair> GetSettableOptions(object optionsInstance)
        {
            Dictionary<string, PropertyInfoSwitchAttributePair> options = new Dictionary<string, PropertyInfoSwitchAttributePair>();

            if (optionsInstance == null)
                return options;

            PropertyInfo[] props = optionsInstance.GetType().GetProperties();
            foreach (PropertyInfo prop in props)
            {
                object[] attrs = prop.GetCustomAttributes(typeof (SwitchAttribute), true);
                if (attrs == null || attrs.Length == 0)
                    continue;

                SwitchAttribute swAttr = (SwitchAttribute) attrs[0];
                PropertyInfoSwitchAttributePair pair = new PropertyInfoSwitchAttributePair(prop, swAttr);
                options.Add(swAttr.Name, pair);
            }

            return options;
        }

        public IList<string> RawArgs
        {
            get { return new ReadOnlyCollection<string>(rawArgs); }
        }

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

            string[] parts = rawArg.Split(new char[] {':', '='});
            if (parts == null || parts.Length == 0 || parts.Length > 2)
            {
                throw new CmdLineParserException(string.Format(
                    "Did not find any value specified for the flag {0}", switchAttr.Name));
            }

            string val;

            if (parts.Length == 1)
            {
                if (propertyInfo.PropertyType == typeof(bool))
                    val = true.ToString();
                else
                {
                    throw new CmdLineParserException(string.Format(
                        "Expected a value associated with flag {0}, but no value was" +
                        "associated with the flag.",
                        switchAttr.Name));
                }
            }
            else
                val = parts[1];

            IPropertySetter setter = setterStrategies[propertyInfo.PropertyType];
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

        private string FindCommandLineArg(string flag)
        {
            return rawArgs.Find(delegate(string arg)
                {
                    string argWithoutSwitch = StripSwitchChar(arg);
                    return argWithoutSwitch.StartsWith(flag, StringComparison.OrdinalIgnoreCase);
                });
        }

        private static string StripSwitchChar(string arg)
        {
            if (string.IsNullOrEmpty(arg))
                return "";

            return arg.Substring(1);
        }
    }
}