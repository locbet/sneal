﻿using System;
using System.Text.RegularExpressions;

namespace SimianMsBuildFormatter
{
    public class OutputFormatter
    {
        public void WriteSimianOutputLine(string line)
        {
            // \s+Between\slines\s(?<line1>\d+)\sand\s(?<line2>\d+)\sin\s(?<file>[\b_a-zA-Z0-9:\\.]+)
            const string regex = @"\s+Between\slines\s(?<line1>\d+)\sand\s(?<line2>\d+)\sin\s(?<file>[\b_a-zA-Z0-9:\\.]+)";
            const RegexOptions options = RegexOptions.IgnorePatternWhitespace | RegexOptions.Multiline | RegexOptions.IgnoreCase;

            var reg = new Regex(regex, options);
            Match match = reg.Match(line);
            Console.WriteLine(match.Success ? FormatFilePathMatch(match) : line);
        }

        private static string FormatFilePathMatch(Match match)
        {
            Group line1Grp = match.Groups["line1"];
            Group line2Grp = match.Groups["line2"];
            Group filePathGrp = match.Groups["file"];

            if (line1Grp.Success && line2Grp.Success && filePathGrp.Success)
            {
                // expected MSBuild clickable output format:
                // d:\source\spikes\SimianRunner\SimianRunner\Runner.cs(51,35): error CS0103: The name 'lie' does not exist in the current context
                string line = string.Format("{0}({1},0): found duplicate lines between {1} and {2}",
                    filePathGrp.Value, line1Grp.Value, line2Grp.Value);

                return line;
            }

            return "";
        }
    }
}
