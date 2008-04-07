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

namespace Sneal.CmdLineParser
{
    public class CommandLineParser
    {
        private List<string> rawArgs;

        /// <summary>
        /// Constructs a new command line parser instance.
        /// </summary>
        /// <param name="rawArgs">The raw command line arguments, usually from main</param>
        public CommandLineParser(IEnumerable<string> rawArgs)
        {
            this.rawArgs = new List<string>(rawArgs);
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
            List<string> args = new List<string>(rawArgs);

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

        private static void SetArgValue(string rawArg, object optionsInstance, PropertyInfoSwitchAttributePair propAttrPair)
        {
            PropertyInfo propertyInfo = propAttrPair.PropertyInfo;
            SwitchAttribute switchAttr = propAttrPair.SwitchAttribute;

            string[] parts = rawArg.Split(new char[] {':', '='});
            if (parts == null || parts.Length == 0 || parts.Length > 2)
            {
                throw new CmdLineParserException(string.Format(
                    "Did not find any value specified for the flag {0}", switchAttr.Name));
            }

            if (parts.Length == 1 && propertyInfo.PropertyType == typeof(bool))
            {
                propertyInfo.SetValue(optionsInstance, true, null);
                return;
            }
            else if (parts.Length != 2)
            {
                throw new CmdLineParserException(string.Format(
                    "Expected a value associated with flag {0}",
                    switchAttr.Name));                
            }

            string val = parts[1];

            if (propertyInfo.PropertyType == typeof(int))
            {
                int iVal;
                if (!Int32.TryParse(val, out iVal))
                {
                    throw new CmdLineParserException(string.Format(
                        "The command line argument for flag {0} was not an integer.  {1}",
                        switchAttr.Name, switchAttr.Description));
                }

                propertyInfo.SetValue(optionsInstance, iVal, null);    
            }
            else if (propertyInfo.PropertyType == typeof(string))
            {
                propertyInfo.SetValue(optionsInstance, val, null);
            }
            else if (propertyInfo.PropertyType == typeof(bool))
            {
                bool bVal;
                if (!Boolean.TryParse(val, out bVal))
                {
                    throw new CmdLineParserException(string.Format(
                        "The command line argument for flag {0} was not a boolean.  {1}",
                        switchAttr.Name, switchAttr.Description));
                }

                propertyInfo.SetValue(optionsInstance, bVal, null);                     
            }
            else
            {
                throw new CmdLineParserException(string.Format(
                    "The data type {0} for property {1} is not supported for flag {2}.",
                    propertyInfo.PropertyType, propertyInfo.Name, switchAttr.Name));
            }
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

        public class PropertyInfoSwitchAttributePair
        {
            public PropertyInfoSwitchAttributePair(PropertyInfo propertyInfo, SwitchAttribute switchAttribute)
            {
                PropertyInfo = propertyInfo;
                SwitchAttribute = switchAttribute;
            }

            public PropertyInfo PropertyInfo;
            public SwitchAttribute SwitchAttribute;
        }
    }
}