#region license
// Copyright 2009 Shawn Neal (neal.shawn@gmail.com)
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
using System.IO;
using System.Text;

namespace Sneal.CmdLineParser
{
    public class DefaultUsageFormatter : IUsageFormatter
    {
        public int DefaultMinTabSize = 20;

        public virtual string GetUsage(ICollection<Option> options)
        {
            var usage = new StringBuilder();
            int firstColumnWidth = GetFirstColumnWidth(options);
            foreach (Option option in options)
            {
                WriteOptionName(usage, option, firstColumnWidth);
                WriteOptionDescription(usage, option, firstColumnWidth);
                usage.AppendLine();
            }
            return usage.ToString();
        }

        private void WriteOptionDescription(StringBuilder usage, Option option, int firstColumnWidth)
        {
            string description = GetFullDescription(option);

            int curLineCharCount = firstColumnWidth;
            string[] words = description.Split(new[] { ' ' });
            for (int i = 0; i < words.Length; ++i)
            {
                // wrap before appending next word if too long for current line
                if (curLineCharCount + words[i].Length >= WrapLinesAtCharNum)
                {
                    usage.AppendFormat("\r\n{0}{1}", WhiteSpace(firstColumnWidth), words[i]);
                    curLineCharCount = firstColumnWidth + words[i].Length;
                }
                else
                {
                    // add a space if not first word, then add current word
                    if (i > 0)
                    {
                        usage.Append(" ");
                        curLineCharCount++;
                    }
                    usage.Append(words[i]);
                    curLineCharCount += words[i].Length;
                }
            }
        }

        /// <summary>
        /// Appends the option's short syntax if the option has both long and 
        /// short forms.
        /// </summary>
        private static string GetFullDescription(Option option)
        {
            string description = option.Description ?? "";
            if (!string.IsNullOrEmpty(option.LongName) && !string.IsNullOrEmpty(option.ShortName))
            {
                description += string.Format(" (Short form: /{0})", option.ShortName);
            }
            return description;
        }

        private void WriteOptionName(StringBuilder usage, Option option, int firstColumnWidth)
        {
            string optionName = "  /" + option.Name;
            usage.Append(optionName);
            usage.Append(WhiteSpace(firstColumnWidth - optionName.Length));
        }

        protected virtual string WhiteSpace(int numTabs)
        {
            var b = new StringBuilder();
            for (int i = 0; i < numTabs; ++i)
            {
                b.Append(" ");
            }
            return b.ToString();
        }

        protected virtual int GetFirstColumnWidth(ICollection<Option> options)
        {
            int len = DefaultMinTabSize;
            foreach (Option option in options)
            {
                int optionLengthWithPadding = option.Name.Length + "  /".Length;
                if (optionLengthWithPadding >= len)
                {
                    len = optionLengthWithPadding + 1;
                }
            }
            return len;
        }

        protected virtual int WrapLinesAtCharNum
        {
            get
            {
                try
                {
                    return Console.BufferWidth;
                }
                catch (IOException)
                {
                    return 80;
                }
            }
        }
    }
}
